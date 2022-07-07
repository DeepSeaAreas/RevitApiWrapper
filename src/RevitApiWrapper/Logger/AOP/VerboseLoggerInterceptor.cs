using Castle.DynamicProxy;
using RevitApiWrapper.Logger.AOP.Interface;
using RevitApiWrapper.Logger.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitApiWrapper.Logger.AOP
{
    /// <summary>
    /// Logger Interceptor
    /// 代码流程日志拦截器
    /// </summary>
    public class VerboseLoggerInterceptor : LoggerInterceptor, IInterceptor
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
        public VerboseLoggerInterceptor(ILogger logger):base(logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Intercept Method
        /// 拦截方法
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            //Default Action
            Action action = () => invocation.Proceed();

            action = AddVerboseLog(action, invocation);

            //Finally Action Doing
            action.Invoke();
        }

        private Action AddVerboseLog(Action action, IInvocation invocation)
        {
            return () =>
            {
                _logger.Verbose($"Do:{invocation.Method.Name}");
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                action.Invoke();

                stopwatch.Stop();
                _logger.Verbose($"Level:{invocation.Method.Name} 耗时：{stopwatch.ElapsedMilliseconds}ms");
            };
        }
    }
}
