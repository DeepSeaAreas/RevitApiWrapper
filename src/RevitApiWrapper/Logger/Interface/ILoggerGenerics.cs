using System;
using System.Collections.Generic;
using System.Text;

namespace RevitApiWrapper.Logger.Interface
{
    /// <summary>
    /// A Logger instance which should be created by RevitApiWrapper.Logger.Interface.ILoggerFactory
    /// 泛型日志记录器，应该被日志工厂创建实例
    /// </summary>
    public interface ILogger<out T> : ILogger where T : class
    {

    }
}
