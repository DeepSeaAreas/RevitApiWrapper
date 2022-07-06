using System;
using System.Collections.Generic;
using System.Text;

namespace RevitApiWrapper.Logger.Model
{
    /// <summary>
    /// Logger Level
    /// 日志等级
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Anything and everything you might want to know about a running block of code.
        /// 详细
        /// </summary>
        Verbose,
        /// <summary>
        /// Internal system events that aren't necessarily observable from the outside.
        /// 调试
        /// </summary>
        Debug,
        /// <summary>
        /// The lifeblood of operational intelligence - things happen.
        /// 信息
        /// </summary>
        Information,
        /// <summary>
        /// Service is degraded or endangered.
        /// 警告
        /// </summary>
        Warning,
        /// <summary>
        /// Functionality is unavailable, invariants are broken or data is lost.
        /// 错误
        /// </summary>
        Error,
        /// <summary>
        /// If you have a pager, it goes off when one of these occurs.
        /// 严重
        /// </summary>
        Fatal
    }
}
