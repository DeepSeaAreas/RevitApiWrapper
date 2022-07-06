using Castle.DynamicProxy;
using RevitApiWrapper.Logger.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace RevitApiWrapper.Logger.AOP.Interface
{
    /// <summary>
    /// Logger Interceptor
    /// 日志拦截器
    /// </summary>
    public abstract class LoggerInterceptor
    {
        /// <summary>
        /// 日志记录器实例
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        public LoggerInterceptor(ILogger logger)
        {
            _logger=logger;
        }
    }
}
