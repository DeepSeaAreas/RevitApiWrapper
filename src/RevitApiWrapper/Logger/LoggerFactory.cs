using System;
using System.Collections.Generic;
using System.Text;
using RevitApiWrapper.Logger.Interface;

namespace RevitApiWrapper.Logger
{
    /// <summary>
    /// Represents a logging factory to provides RevitApiWrapper.Logger.Logger<T> instance
    /// 日志工厂，提供日志记录
    /// </summary>
    public class LoggerFactory : ILoggerFactory
    {
        /// <summary>
        /// Singleton Logger Map
        /// 日志单例列表
        /// </summary>
        static Dictionary<string, ILogger> loggerMap = new Dictionary<string, ILogger>();

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="configuration">日志配置</param>
        private LoggerFactory(ILoggerConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Statically create logger factory method
        /// 静态创建日志工厂方法
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static ILoggerFactory Build(ILoggerConfiguration configuration) => new LoggerFactory(configuration);

        /// <summary>
        /// Logger Configuration
        /// 日志配置
        /// </summary>
        private ILoggerConfiguration Configuration { get; }

        /// <summary>
        /// Create a logging instance based on class
        /// 创建日志记录实例
        /// </summary>
        /// <typeparam name="T">被记录的类</typeparam>
        /// <returns></returns>
        public ILogger<T> CreateLogger<T>() where T : class
        {
            if (!loggerMap.ContainsKey(typeof(T).FullName))
            {
                ILogger<T> logger = Logger<T>.CreateNew(Configuration);
                loggerMap.Add(typeof(T).FullName, logger);
                return logger;
            }
            return (ILogger<T>)loggerMap[typeof(T).FullName];
        }
    }
}
