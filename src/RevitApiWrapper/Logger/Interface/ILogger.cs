using System;
using System.Collections.Generic;
using System.Text;
using RevitApiWrapper.Logger.Model;

namespace RevitApiWrapper.Logger.Interface
{
    /// <summary>
    /// A Logger instance which should be created by RevitApiWrapper.Logger.Interface.ILoggerFactory
    /// 日志记录器，应该被日志工厂创建实例
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// logger configuration
        /// 日志配置
        /// </summary>
        ILoggerConfiguration Configuration { get; set; }

        /// <summary>
        /// logger event
        /// 日志记录事件
        /// </summary>
        event EventHandler<LogMessage> ToLogger;

        /// <summary>
        /// Triggers logging events based on the specified log level
        /// 根据指定的日志级别触发日志记录事件
        /// </summary>
        /// <param name="level">日志等级</param>
        /// <param name="message">信息</param>
        /// <param name="exception">异常</param>
        void Write(LogLevel level, string message, Exception exception = null);

        /// <summary>
        /// Workflow tracking or verbose level logging to triggers logging events
        /// 工作流追踪或详细级别日志触发记录事件
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="exception">异常</param>
        void Verbose(string message, Exception exception = null);
        
        /// <summary>
        /// Debug level logging to triggers logging events
        /// 调试级别日志触发记录事件
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Debug(string message, Exception exception = null);

        /// <summary>
        /// Information level logging to triggers logging events
        /// 信息级别日志触发记录事件
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Information(string message, Exception exception = null);

        /// <summary>
        /// Warning level logging to triggers logging events
        /// 警告级别日志触发记录事件
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Warning(string message, Exception exception = null);

        /// <summary>
        /// Error level logging to triggers logging events
        /// 错误级别日志触发记录事件
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Error(string message, Exception exception = null);

        /// Fatal level logging to triggers logging events
        /// 严重级别日志触发记录事件
        void Fatal(string message, Exception exception = null);
    }
}
