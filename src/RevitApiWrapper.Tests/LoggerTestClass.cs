using RevitApiWrapper.Logger;
using RevitApiWrapper.Logger.AOP;
using RevitApiWrapper.Logger.AOP.Extension;
using RevitApiWrapper.Logger.Extension;
using RevitApiWrapper.Logger.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitApiWrapper.Tests
{
    public class LoggerTestClass
    {
        private readonly ILogger _logger;

        public static ILoggerFactory LogFactory { get; set; }

        public LoggerTestClass()
        {
            ILoggerConfiguration config = LoggerConfiguration.Default.FromJson();
            LogFactory = LoggerFactory.Build(config);
            _logger=LogFactory.CreateLogger<LoggerTestClass>()
                              .AOP(LoggerAOPHelper.LoadAOPJson());
        }

        public void logTest()
        {
            _logger.Information("this is a test");
        }
    }
}
