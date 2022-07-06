using System;
using System.Collections.Generic;
using System.Text;
using RevitApiWrapper.Logger.Interface;
using RevitApiWrapper.Logger.FileActuator.Model;
using RevitApiWrapper.Logger.FileActuator.Format;
using RevitApiWrapper.Logger.Model;
using System.IO;
using System.Reflection;

namespace RevitApiWrapper.Logger.FileActuator
{
    /// <summary>
    /// Logger Event Actuator
    /// 日志记录事件执行器
    /// </summary>
    public class Actuator : LoggerEvent
    {
        /// <summary>
        /// 日志文件地址
        /// </summary>
        public string FilePath { get; set; } = "logs/log.txt";
        
        /// <summary>
        /// 日志输出模板
        /// </summary>
        public string OutputTemplate { get; set; } = "{LoggerTime:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";
        
        /// <summary>
        /// 写入周期，默认为不分周期
        /// </summary>
        public WrittenInterval Interval { get; set; } = WrittenInterval.None;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="args"></param>
        public Actuator(Dictionary<string, string> args):base(args)
        {
            if (args.ContainsKey("Path"))
            {
                FilePath = args["Path"];
            }

            if (args.ContainsKey("OutputTemplate"))
            {
                OutputTemplate = args["OutputTemplate"];
            }

            if (args.ContainsKey("Interval"))
            {
                string interval = args["Interval"];
                if (Enum.TryParse(interval, out WrittenInterval _interval))
                {
                    Interval = _interval;
                }
            }

            MessageTemplate = new OutputTemplateFormat(OutputTemplate);
            MessageTemplate.Format();
        }

        /// <summary>
        /// Output Template Fromat
        /// 无参数化输出模板
        /// </summary>
        private OutputTemplateFormat MessageTemplate { get; set; }

        /// <summary>
        /// To Logger
        /// 记录日志
        /// </summary>
        /// <param name="logEvent"></param>
        public override void ToLogger(LogMessage logEvent)
        {
            //Message
            List<string> args = new List<string>();
            foreach (var item in MessageTemplate.PropertyTemplates)
            {
                string arg = "None";

                if (item.Key == "NewLine")
                {
                    arg = Environment.NewLine;
                }
                else
                {
                    var property = typeof(LogMessage).GetProperty(item.Key);
                    if (property != null)
                    {
                        var data = property.GetValue(logEvent);
                        if (data != null)
                        {
                            if (!string.IsNullOrWhiteSpace(item.Value))
                            {
                                arg = string.Format("{0:" + item.Value + "}", data);
                            }
                            else
                            {
                                arg = data.ToString();
                            }
                        }
                    }
                }

                args.Add(arg);
            }
            string message = string.Format(MessageTemplate.MessageTemplate, args.ToArray());

            //File Name
            string suf = string.Empty;
            switch (Interval)
            {
                case WrittenInterval.None:
                    break;

                case WrittenInterval.Day:
                    suf = DateTime.Now.ToString("yyyyMMdd");
                    break;

                case WrittenInterval.Week:
                    suf = $"{DateTime.Now.Year}_{new System.Globalization.GregorianCalendar().GetWeekOfYear(DateTime.Now, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString("00")}week";
                    break;

                case WrittenInterval.Month:
                    suf = DateTime.Now.ToString("yyyyMM");
                    break;
            }

            string baseRoot = Path.GetDirectoryName(FilePath);
            string fileName = string.Format("{0}_{1}", Path.GetFileNameWithoutExtension(FilePath), suf);
            string extension = Path.GetExtension(FilePath);

            string fullFile = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().Location).LocalPath), baseRoot, fileName + extension);

            //Write File
            if (!Directory.Exists(Path.GetDirectoryName(fullFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullFile));
            }

            if (System.IO.File.Exists(fullFile))
            {
                using (var stream = new StreamWriter(fullFile, true))
                {
                    stream.WriteLine(Environment.NewLine+message);
                }
            }
            else
            {
                System.IO.File.WriteAllText(fullFile, message);
            }
        }
    }
}
