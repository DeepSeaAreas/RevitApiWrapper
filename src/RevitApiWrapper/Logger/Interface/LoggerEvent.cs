using RevitApiWrapper.Logger.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace RevitApiWrapper.Logger.Interface
{
    /// <summary>
    /// Abstract Logger Event
    /// 抽象日志记录事件
    /// </summary>
    public abstract class LoggerEvent
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="args"></param>
        public LoggerEvent(Dictionary<string, string> args)
        {

        }

        /// <summary>
        /// To Logger
        /// 记录事件
        /// </summary>
        /// <param name="logEvent"></param>
        public abstract void ToLogger(LogMessage logEvent);
    }
}
