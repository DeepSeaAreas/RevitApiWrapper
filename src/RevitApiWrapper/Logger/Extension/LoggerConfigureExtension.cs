using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using RevitApiWrapper.Logger.Interface;
using RevitApiWrapper.Logger.Model;

namespace RevitApiWrapper.Logger.Extension
{
    /// <summary>
    /// Logger Configuration Extension
    /// 日志配置扩展方法
    /// </summary>
    public static class LoggerConfigureExtension
    {
        /// <summary>
        /// Read the logger configuration from the local Json file
        /// 从本地Json配置文件读取日志配置
        /// </summary>
        /// <param name="loggerConfigure">日志配置实例</param>
        /// <param name="fileName">json文件名</param>
        /// <param name="nodeName">节点名</param>
        /// <returns></returns>
        public static ILoggerConfiguration FromJson(this ILoggerConfiguration loggerConfigure, string fileName= "LoggerSetting.json", string nodeName = "RAWLogger")
        {
            var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().Location).LocalPath))
                .AddJsonFile(fileName)
                .Build();

            var configureSection = configuration.GetSection(nodeName);
            return loggerConfigure.BuildOfJsonConfigure(configureSection);
        }

        /// <summary>
        /// Read the logger configuration from the configuration section
        /// 从配置节点读取日志配置
        /// </summary>
        /// <param name="configuration">日志配置实例</param>
        /// <param name="section">配置节点</param>
        /// <returns></returns>
        private static ILoggerConfiguration BuildOfJsonConfigure(this ILoggerConfiguration configuration, IConfigurationSection section)
        {
            return configuration.MinimumLevelofConfigration(section).LevelOverrideofConfigration(section).RegisterActionofConfigration(section);
        }

        /// <summary>
        /// Sets the minimum logger level from the configuration section
        /// 从配置节点设置最小日志记录等级
        /// </summary>
        /// <param name="configuration">日志配置实例</param>
        /// <param name="configureSection">配置节点</param>
        /// <returns></returns>
        private static ILoggerConfiguration MinimumLevelofConfigration(this ILoggerConfiguration configuration, IConfigurationSection configureSection)
        {
            var levelSection = configureSection.GetSection("MinimumLevel");
            var levelDefaultSection = ((levelSection.Value != null) ? levelSection : levelSection.GetSection("Default"));
            if (levelDefaultSection.Value != null)
            {
                if (Enum.TryParse<LogLevel>(levelDefaultSection.Value, out LogLevel minimumLevel))
                {
                    return configuration.SetMinimumLevel(minimumLevel);
                }
            }
            return configuration;
        }

        /// <summary>
        /// Sets logger level override from the configuration section
        /// 从配置节点设置日志等级覆盖
        /// </summary>
        /// <param name="configuration">日志配置实例</param>
        /// <param name="configureSection">配置节点</param>
        /// <returns></returns>
        private static ILoggerConfiguration LevelOverrideofConfigration(this ILoggerConfiguration configuration, IConfigurationSection configureSection)
        {
            Dictionary<string, LogLevel> overrides = new Dictionary<string, LogLevel>();
            foreach (var overrideItem in configureSection.GetSection("MinimumLevel:Override").GetChildren())
            {
                string overrideKey = overrideItem.Key;
                string overrideValue = overrideItem.Value;
                if (Enum.TryParse<LogLevel>(overrideValue, out LogLevel overrideLevel))
                {
                    overrides.Add(overrideKey, overrideLevel);
                }
            }
            return configuration.SetOverride(overrides);
        }

        /// <summary>
        /// Registry logger delegate events from the configuration section 
        /// 从配置节点注册日志记录委托事件
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="configureSection"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static ILoggerConfiguration RegisterActionofConfigration(this ILoggerConfiguration configuration, IConfigurationSection configureSection)
        {
            var actionSection = configureSection.GetSection("Extension");
            if (actionSection.GetChildren().Any())
            {
                foreach (var section in actionSection.GetChildren())
                {
                    //反射
                    var assemblySection = section.GetSection("AssemblyName");
                    var typeSection = section.GetSection("TypeName");
                    string assemblyName = assemblySection.Value;
                    string typeName = typeSection.Value;
                    if (string.IsNullOrWhiteSpace(assemblyName) || string.IsNullOrWhiteSpace(typeName))
                    {
                        throw new InvalidOperationException("A zero-length or whitespace assembly/type/methodName name was provided");
                    }

                    //构造参数
                    var argsSection = section.GetSection("Args");
                    Dictionary<string, string> args = null;
                    if (argsSection.GetChildren().Any())
                    {
                        args = new Dictionary<string, string>();
                        foreach (var item in argsSection.GetChildren())
                        {
                            if (args.ContainsKey(item.Key))
                            {
                                args[item.Key] = item.Value;
                            }
                            else
                            {
                                args.Add(item.Key, item.Value);
                            }
                        }
                    }

                    Assembly assembly = Assembly.Load(assemblyName);
                    Type type = assembly.GetType(typeName);
                    if (!typeof(LoggerEvent).IsAssignableFrom(type))
                    {
                        throw new InvalidOperationException("A invalid logger event");
                    }
                    object instance = Activator.CreateInstance(type, args);

                    //公共属性
                    var propertiesSection = section.GetSection("Properties");
                    Dictionary<string, string> properties = new Dictionary<string, string>();
                    if (propertiesSection.GetChildren().Any())
                    {
                        foreach (var item in propertiesSection.GetChildren())
                        {
                            if (properties.ContainsKey(item.Key))
                            {
                                properties[item.Key] = item.Value;
                            }
                            else
                            {
                                properties.Add(item.Key, item.Value);
                            }
                        }
                    }

                    if (instance != null)
                    {
                        foreach (var item in properties)
                        {
                            var property = type.GetProperty(item.Key);
                            if (property != null)
                            {
                                property.SetValue(instance, item.Value);
                            }
                        }

                        return configuration.AddAction((logEvent) =>
                        {
                            type.GetMethod("ToLogger").Invoke(instance, new object[] { logEvent });
                        });
                    }
                }
            }
            return configuration;
        }
    }

}
