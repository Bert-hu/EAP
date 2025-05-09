using EAP.Client.Forms;
using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using EAP.Client.Utils;
using log4net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Secs4Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class ReelCompletedEvent : IEventHandler
    {
        private readonly ILog traLog = LogManager.GetLogger("Trace");

        private readonly RabbitMqService rabbitMqService;
        private readonly ISecsGem secsGem;
        private readonly IConfiguration configuration;

        public ReelCompletedEvent(RabbitMqService rabbitMqService, ISecsGem secsGem, IConfiguration configuration)
        {
            this.rabbitMqService = rabbitMqService;
            this.secsGem = secsGem;
            this.configuration = configuration;
        }

        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            try
            {
                traLog.Info($"Reel完成。");
                MainForm.Instance.ClearCount();
            }
            catch (Exception ex)
            {
                traLog.Error(ex.ToString());
            }
        }
    }
}
