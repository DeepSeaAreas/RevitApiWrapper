using System;
using System.Collections.Generic;
using System.Text;
using RevitApiWrapper.Logger.Interface;
using RevitApiWrapper.Logger.Model;

namespace RevitApiWrapper.Logger
{
    /// <summary>
    /// Logger Configuration
    /// 日志配置
    /// </summary>
    public class LoggerConfiguration : ILoggerConfiguration
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        private LoggerConfiguration()
        {

        }

        /// <summary>
        /// Default logger configuration
        /// 默认日志配置
        /// </summary>
        public static ILoggerConfiguration Default => new LoggerConfiguration();

        /// <summary>
        /// Minimum log level
        /// 最小记录日志级别
        /// </summary>
        public LogLevel MinimumLevel { get; set; } = LogLevel.Information;

        /// <summary>
        /// Log level override
        /// 日志级别覆盖
        /// </summary>
        public IList<KeyValuePair<string, LogLevel>> OverrideLevels { get; set; } = new List<KeyValuePair<string, LogLevel>>();

        /// <summary>
        /// A collection of logging event delegates
        /// 日志记录事件委托集合
        /// </summary>
        public IList<Action<LogMessage>> LoggedActions { get; set; } = new List<Action<LogMessage>>();

        /// <summary>
        /// Set minimum log level
        /// 设置最小记录日志级别
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <returns></returns>
        public ILoggerConfiguration SetMinimumLevel(LogLevel level)
        {
            MinimumLevel = level;
            return this;
        }

        /// <summary>
        /// Add log level override
        /// 添加日志级别覆盖
        /// </summary>
        /// <param name="overrides"></param>
        /// <returns></returns>
        public ILoggerConfiguration SetOverride(Dictionary<string, LogLevel> overrides)
        {
            foreach (var item in overrides)
            {
                OverrideLevels.Add(item);
            }
            return this;
        }

        /// <summary>
        /// <summary>
        /// Register logging events
        /// 注册日志记录事件
        /// </summary>
        /// </summary>
        /// <param name="action">日志记录触发事件</param>
        /// <returns></returns>
        public ILoggerConfiguration AddAction(Action<LogMessage> action)
        {
            LoggedActions.Add(new Action<LogMessage>((e) =>
            {
                action.Invoke(e);
            }));
            return this;
        }
    }
}
