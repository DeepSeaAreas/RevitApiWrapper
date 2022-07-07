using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RevitApiWrapper.Logger.FileActuator.Format
{
    /// <summary>
    /// Output Template Fromat
    /// 无参数化输出模板
    /// </summary>
    public class OutputTemplateFormat
    {
        /// <summary>
        /// Parameterized message
        /// 参数化输出信息
        /// </summary>
        string _messageTemplate;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="messageTemplate"></param>
        public OutputTemplateFormat(string messageTemplate)
        {
            _messageTemplate = messageTemplate;
        }

        /// <summary>
        /// Property List
        /// 参数列表
        /// </summary>
        public Dictionary<string, string> PropertyTemplates { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Parameterized message template
        /// 参数化信息模板
        /// </summary>
        public string MessageTemplate { get; set; }

        /// <summary>
        /// Without parameterized
        /// 无参数化
        /// </summary>
        public void Format()
        {
            PropertyTemplates.Clear();
            StringBuilder msgBuilder = new StringBuilder();
            List<StringBuilder> propertyBuilders = new List<StringBuilder>();

            int startAt = 0;
            do
            {
                char c = _messageTemplate[startAt];
                msgBuilder.Append(c);

                if (c == '{')
                {
                    var propertyBuilder = ParseProperty(startAt + 1, out int next);
                    if (propertyBuilder != null)
                    {
                        propertyBuilders.Add(propertyBuilder);
                        msgBuilder.Append(propertyBuilders.Count - 1);
                    }

                    startAt = next;
                    continue;
                }

                startAt++;
            }
            while (startAt < _messageTemplate.Length);

            MessageTemplate = msgBuilder.ToString();

            foreach (var template in propertyBuilders)
            {
                string data = template.ToString();
                if (data.Contains(':'))
                {
                    int split = data.IndexOf(':');
                    PropertyTemplates.Add(data.Substring(0, split), data.Substring(split + 1));
                }
                else
                {
                    PropertyTemplates.Add(data, String.Empty);
                }
            }
        }

        /// <summary>
        /// Get property form parameterized message
        /// 提取参数
        /// </summary>
        /// <param name="startAt"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        private StringBuilder ParseProperty(int startAt, out int next)
        {
            StringBuilder propertyBuilder = new StringBuilder();
            next = startAt;
            do
            {
                char c = _messageTemplate[startAt];
                if (c == '}')
                {
                    next = startAt;
                    return propertyBuilder;
                }
                else
                {
                    propertyBuilder.Append(c);
                }

                startAt++;
            }
            while (startAt < _messageTemplate.Length);
            return null;
        }
    }
}
