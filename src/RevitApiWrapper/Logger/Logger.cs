using System;
using System.Collections.Generic;
using System.Text;
using RevitApiWrapper.Logger.Interface;
using RevitApiWrapper.Logger.Model;

namespace RevitApiWrapper.Logger
{
    /// <summary>
    /// A Logger instance which should be created by RevitApiWrapper.Logger.Interface.ILoggerFactory
    /// 日志记录器，应该被日志工厂创建实例
    /// </summary>
    public class Logger<T> : ILogger, ILogger<T> where T : class
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="configuration">日志配置</param>
        internal Logger(ILoggerConfiguration configuration)
        {
            Configuration = configuration;
            foreach (var action in Configuration.LoggedActions)
            {
                ToLogger += new EventHandler<LogMessage>((sender, e) =>
                {
                    action.Invoke(e);
                });
            }
        }

        /// <summary>
        /// Statically create logger method
        /// 静态创建日志记录器方法
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static Logger<T> CreateNew(ILoggerConfiguration configuration)
        {
            return new Logger<T>(configuration);
        }

        /// <summary>
        /// Source Context Class
        /// 被记录日志的类
        /// </summary>
        private T t { get; set; } = default(T);

        /// <summary>
        /// logger configuration
        /// 日志配置
        /// </summary>
        public ILoggerConfiguration Configuration { get; set; }

        /// <summary>
        /// logger event
        /// 日志记录事件
        /// </summary>
        public event EventHandler<LogMessage> ToLogger;

        /// <summary>
        /// Triggers logging events based on the specified log level
        /// 根据指定的日志级别触发日志记录事件
        /// </summary>
        /// <param name="level">日志等级</param>
        /// <param name="message">信息</param>
        /// <param name="exception">异常</param>
        public void Write(LogLevel level, string message, Exception exception = null)
        {
            LogLevel _level = level;

            //Override Level
            string typeName = typeof(T).FullName;
            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new InvalidOperationException("A zero-length or whitespace type name was provided");
            }

            foreach (var item in Configuration.OverrideLevels)
            {
                if (!string.IsNullOrWhiteSpace(item.Key) && typeName.Contains(item.Key))
                {
                    _level = item.Value;
                }
            }

            //Minimum Level and Logger Action
            if ((int)_level >= ((int)Configuration.MinimumLevel))
            {
                LogMessage logEvent = new LogMessage(typeof(T), _level, message, exception);
                ToLogger.Invoke(this, logEvent);
            }
        }

        /// <summary>
        /// Workflow tracking or verbose level logging to triggers logging events
        /// 工作流追踪或详细级别日志触发记录事件
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="exception">异常</param>
        public void Verbose(string message, Exception exception = null)
        {
            Write(LogLevel.Verbose, message, exception);
        }

        /// <summary>
        /// Debug level logging to triggers logging events
        /// 调试级别日志触发记录事件
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Debug(string message, Exception exception = null)
        {
            Write(LogLevel.Debug, message, exception);
        }

        /// <summary>
        /// Information level logging to triggers logging events
        /// 信息级别日志触发记录事件
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Information(string message, Exception exception = null)
        {
            Write(LogLevel.Information, message, exception);
        }

        /// <summary>
        /// Warning level logging to triggers logging events
        /// 警告级别日志触发记录事件
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Warning(string message, Exception exception = null)
        {
            Write(LogLevel.Warning, message, exception);
        }

        /// <summary>
        /// Error level logging to triggers logging events
        /// 错误级别日志触发记录事件
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Error(string message, Exception exception = null)
        {
            Write(LogLevel.Error, message, exception);
        }

        /// Fatal level logging to triggers logging events
        /// 严重级别日志触发记录事件
        public void Fatal(string message, Exception exception = null)
        {
            Write(LogLevel.Fatal, message, exception);
        }
    }
}
