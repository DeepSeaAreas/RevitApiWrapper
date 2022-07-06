using Castle.DynamicProxy;
using Microsoft.Extensions.Configuration;
using RevitApiWrapper.Logger.AOP.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RevitApiWrapper.Logger.AOP
{
    /// <summary>
    /// Logger Aop Helper
    /// 日志拦截帮助类
    /// </summary>
    public class LoggerAOPHelper
    {
        /// <summary>
        /// Get interceptors from Json files
        /// 从Json文件获取拦截器字典
        /// </summary>
        /// <param name="fileName">Json文件名</param>
        /// <param name="nodeName">节点名</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Dictionary<string, string> LoadAOPJson(string fileName= "LoggerAOP.json",string nodeName= "RAWLoggerAOP")
        {
            var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().Location).LocalPath))
                .AddJsonFile(fileName)
                .Build();

            var configureSection = configuration.GetSection(nodeName);

            Dictionary<string, string> items = new Dictionary<string, string>();
            foreach (var item in configureSection.GetChildren())
            {
                if(items.ContainsKey(item.Key))
                {
                    items[item.Key]=item.Value;
                }
                else
                {
                    items.Add(item.Key, item.Value);
                }
            }

            return items;
        }
    }
}
