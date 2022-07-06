using System;
using System.Collections.Generic;
using System.Text;
using RevitApiWrapper.Logger.Interface;
using RevitApiWrapper.Logger.FileActuator.Model;

namespace RevitApiWrapper.Logger.FileActuator.Extension
{
    /// <summary>
    /// Logger Configure Extension
    /// 日志配置扩展
    /// </summary>
    public static class LoggerConfigureExtension
    {
        /// <summary>
        /// Write to file log
        /// 写入文件日志
        /// </summary>
        /// <param name="configure">日志配置实例</param>
        /// <param name="path">日志文件地址</param>
        /// <param name="outputTemplate">日志输出模板</param>
        /// <param name="interval">日志文件分隔周期</param>
        /// <returns></returns>
        public static ILoggerConfiguration AddFile(this ILoggerConfiguration configure, string path = "logs/log.txt", string outputTemplate = "{LoggerTime:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}", WrittenInterval interval = WrittenInterval.None)
        {
            var args = new Dictionary<string, string>();
            args.Add("Path", path);
            args.Add("OutputTemplate", outputTemplate);
            args.Add("Interval", interval.ToString());
            Actuator actuator = new Actuator(args);

            configure.AddAction((logEvent) =>
            {
                actuator.ToLogger(logEvent);
            });

            return configure;
        }
    }
}
