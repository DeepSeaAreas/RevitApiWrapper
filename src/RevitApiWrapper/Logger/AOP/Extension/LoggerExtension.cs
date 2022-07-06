using Castle.DynamicProxy;
using RevitApiWrapper.Logger.AOP.Interface;
using RevitApiWrapper.Logger.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RevitApiWrapper.Logger.AOP.Extension
{
    /// <summary>
    /// Logger Extension
    /// 日志记录器扩展
    /// </summary>
    public static class LoggerExtension
    {
        /// <summary>
        /// Loggger AOP
        /// Logger AOP代理
        /// </summary>
        /// <typeparam name="T">日志类</typeparam>
        /// <param name="logger">日志记录器实例</param>
        /// <param name="args">拦截器字典</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static ILogger<T> AOP<T>(this ILogger<T> logger,Dictionary<string,string> args) where T : class
        {
            ProxyGenerator proxy = new ProxyGenerator();

            IDictionary<string, IInterceptor> interceptors=new Dictionary<string, IInterceptor>();
            foreach (var item in args)
            {
                Assembly assembly = Assembly.Load(item.Value);
                Type type = assembly.GetType(item.Key);

                if (!typeof(LoggerInterceptor).IsAssignableFrom(type) || !typeof(IInterceptor).IsAssignableFrom(type))
                {
                    throw new InvalidOperationException("A invalid logger interceptor");
                }

                IInterceptor instance = (IInterceptor)Activator.CreateInstance(type,new object[] { logger});
                if (instance != null)
                {
                    if (interceptors.ContainsKey(type.FullName))
                    {
                        interceptors[type.FullName] = instance;
                    }
                    else
                    {
                        interceptors.Add(type.FullName, instance);
                    }
                }
            }

            return (ILogger<T>)proxy.CreateInterfaceProxyWithTarget(logger, interceptors.Values.ToArray());
        }
    }
}
