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

        public LoggerTestClass()
        {
            _logger=LoggerFactory.Build(LoggerConfiguration.Default.FromJson())
                .CreateLogger<LoggerTestClass>()
                .AOP(LoggerAOPHelper.LoadAOPJson());

        }

        public void logTest()
        {
            _logger.Information("this is a test");
        }
    }
}
