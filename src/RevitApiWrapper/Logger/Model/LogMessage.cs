using System;
using System.Collections.Generic;
using System.Text;

namespace RevitApiWrapper.Logger.Model
{
    /// <summary>
    /// Logger Message
    /// 日志信息
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="sourceContextType">被记录的类的类型</param>
        /// <param name="level">日志级别</param>
        /// <param name="message">信息</param>
        /// <param name="exception">异常</param>
        public LogMessage(Type sourceContextType, LogLevel level, string message, Exception exception)
        {
            LoggerTime = DateTime.Now;

            SourceContext = sourceContextType.FullName;
            Level = level;
            Message = message;
            Exception = ParseException(exception);
        }

        /// <summary>
        /// Exception Parse
        /// 异常解析
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        private string ParseException(Exception exception)
        {
            string result = string.Empty;
            if (exception == null)
            {
                result = "None";
            }

            while (exception != null)
            {
                result += Environment.NewLine + exception.ToString();
                exception = exception.InnerException;
            }

            return result;
        }

        /// <summary>
        /// Logger Time
        /// 记录时间
        /// </summary>
        public DateTime LoggerTime { get; set; }

        /// <summary>
        /// Name of source context
        /// 被记录的类的类型名称
        /// </summary>
        public string SourceContext { get; set; }
        
        /// <summary>
        /// Logger Level
        /// 日志级别
        /// </summary>
        public LogLevel Level { get; set; }
        
        /// <summary>
        /// Logger Message
        /// 记录信息
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// Exception Information
        /// 异常信息
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// 重写字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{string.Format("{0:yyyy-MM-dd HH:mm:ss.fff zzz}",LoggerTime)} [{Level}] {Message}{Environment.NewLine}{Exception}";
        }

    }
}
