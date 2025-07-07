using EAP.Client.NonSecs.Message;
using EAP.Client.RabbitMq;
using log4net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EAP.Client.NonSecs.PrimaryMessageHandler
{
    public class PrimaryS5F1 : IPrimaryMessageHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        private readonly RabbitMqService rabbitMqService;
        private readonly IConfiguration configuration;

        public PrimaryS5F1(RabbitMqService rabbitMqService, IConfiguration configuration)
        {
            this.rabbitMqService = rabbitMqService;
            this.configuration = configuration;
        }
        public async Task HandlePrimaryMessage(NonSecsMessageWrapper wrapper)
        {
            try
            {
                var s5f2 = new NonSecsMessage(5, 2);
                await wrapper.TryReplyAsync(s5f2);

                var s5f1 = JsonConvert.DeserializeObject<S5F1>(wrapper.PrimaryMessageString);

                // 根据AlarmID首位映射subEQID
                string alarmIdStr = s5f1.AlarmID ?? string.Empty;
                string subEQIndex = alarmIdStr.Length > 0 ? alarmIdStr[..1] : "0";
                var subEqDict = configuration.GetSection("NonSecs:SubEquipment").Get<Dictionary<string, string>>();

                string subEQID = subEqDict.TryGetValue(subEQIndex, out var eqid) ? eqid : "Unknown";

                var equipmentType = configuration.GetSection("Custom")["EquipmentType"];

                var para = new Dictionary<string, object>
            {
                { "AlarmEqp", subEQID },
                { "AlarmCode", s5f1.AlarmID },
                { "AlarmText", s5f1.AlarmText },
                { "AlarmSource", equipmentType },
                { "AlarmTime", DateTime.Now },
                { "AlarmSet", s5f1.AlarmSet == "Y" }
            };

                var trans = new RabbitMqTransaction
                {
                    TransactionName = "EquipmentAlarm",
                    EquipmentID = subEQID,
                    Parameters = para
                };

                rabbitMqService.Produce("EAP.Services", trans);

            }
            catch (Exception ex) {
                dbgLog.Error(ex);
            }
          

        }
    }
}
