using System;
using System.Collections.Generic;
using System.Text;
using RevitApiWrapper.Logger.Model;

namespace RevitApiWrapper.Logger.Interface
{
    /// <summary>
    /// Logger Configuration
    /// 日志配置
    /// </summary>
    public interface ILoggerConfiguration
    {
        /// <summary>
        /// Minimum log level
        /// 最小记录日志级别
        /// </summary>
        LogLevel MinimumLevel { get; set; }

        /// <summary>
        /// Log level override
        /// 日志级别覆盖
        /// </summary>
        IList<KeyValuePair<string, LogLevel>> OverrideLevels { get; set; }

        /// <summary>
        /// A collection of logging event delegates
        /// 日志记录事件委托集合
        /// </summary>
        IList<Action<LogMessage>> LoggedActions { get; set; }

        /// <summary>
        /// Set minimum log level
        /// 设置最小记录日志级别
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <returns></returns>
        ILoggerConfiguration SetMinimumLevel(LogLevel level);

        /// <summary>
        /// Add log level override
        /// 添加日志级别覆盖
        /// </summary>
        /// <param name="overrides"></param>
        /// <returns></returns>
        ILoggerConfiguration SetOverride(Dictionary<string, LogLevel> overrides);

        /// <summary>
        /// <summary>
        /// Register logging events
        /// 注册日志记录事件
        /// </summary>
        /// </summary>
        /// <param name="action">日志记录触发事件</param>
        /// <returns></returns>
        ILoggerConfiguration AddAction(Action<LogMessage> action);
    }
}
