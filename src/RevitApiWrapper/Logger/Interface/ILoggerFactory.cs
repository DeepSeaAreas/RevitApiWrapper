using System;
using System.Collections.Generic;
using System.Text;

namespace RevitApiWrapper.Logger.Interface
{
    /// <summary>
    /// Represents a logging factory to provides RevitApiWrapper.Logger.Interface.ILogger<T> instance
    /// 日志工厂，提供日志记录
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// Create a logging instance based on class
        /// 创建日志记录实例
        /// </summary>
        /// <typeparam name="T">被记录的类</typeparam>
        /// <returns></returns>
        ILogger<T> CreateLogger<T>() where T : class;
    }
}
