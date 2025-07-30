using log4net.Appender;
using log4net.Core;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EAP.Client.Forms
{
    public class RichTextBoxAppender : RollingFileAppender
    {
        public UIRichTextBox RichTextBox { get; set; }

        protected override void Append(LoggingEvent loggingEvent)
        {
            base.Append(loggingEvent);
            if (RichTextBox == null) return;

            // 根据日志级别设置不同的颜色
            Color color = Color.Black;
            switch (loggingEvent.Level.Name)
            {
                case "DEBUG":
                    color = Color.Gray;
                    break;
                case "INFO":
                    color = Color.Green;
                    break;
                case "WARN":
                    color = Color.Orange;
                    break;
                case "ERROR":
                    color = Color.Red;
                    break;
                case "FATAL":
                    color = Color.Magenta;
                    break;
            }

            // 将日志信息格式化为字符串
            string message = RenderLoggingEvent(loggingEvent);
            try
            {
                // 在UI线程上执行委托，将日志信息追加到RichTextBox控件
                RichTextBox.Invoke(new Action(() =>
                {
                    //超过100行清空
                    if (RichTextBox.Lines.Length > 200)
                    {
                        RichTextBox.Clear();
                    }
                    RichTextBox.SelectionStart = RichTextBox.TextLength;
                    RichTextBox.SelectionLength = 0;
                    RichTextBox.SelectionColor = color;
                    RichTextBox.AppendText(message);
                    RichTextBox.SelectionColor = RichTextBox.ForeColor;
                    RichTextBox.ScrollToCaret();

                }));
            }
            catch
            {

            }
        }

    }
}
