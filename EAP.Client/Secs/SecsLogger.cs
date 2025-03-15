using log4net;
using Microsoft.Extensions.Logging;
using Secs4Net;
using Secs4Net.Sml;

namespace EAP.Client.Secs
{
    internal class SecsLogger : ISecsGemLogger
    {

        private ILog secsLogger = LogManager.GetLogger("secsLogger");

        public void MessageIn(SecsMessage msg, int id)
        {
            //如果msg.ToSml()长度大于3000，只打印前3000个字符
            if (msg.ToSml().Length > 3000)
            {
                secsLogger.Info($"<-- [0x{id:X8}]\r\n{msg.ToSml().Substring(0, 3000)}");
            }
            else
            {
                secsLogger.Info($"<-- [0x{id:X8}]\r\n{msg.ToSml()}");
            }
        }
        public void MessageOut(SecsMessage msg, int id)
        {
            //如果msg.ToSml()长度大于3000，只打印前3000个字符
            if (msg.ToSml().Length > 3000)
            {
                secsLogger.Info($"--> [0x{id:X8}]\r\n{msg.ToSml().Substring(0, 3000)}");
            }
            else
            {
                secsLogger.Info($"--> [0x{id:X8}]\r\n{msg.ToSml()}");
            }
        }
        public void Debug(string msg) => secsLogger.Debug(msg);
        public void Info(string msg) => secsLogger.Info(msg);
        public void Warning(string msg) => secsLogger.Warn(msg);
        public void Error(string msg, SecsMessage? message, Exception? ex) => secsLogger.Error($"{msg} {message}\n", ex);
    }
}
