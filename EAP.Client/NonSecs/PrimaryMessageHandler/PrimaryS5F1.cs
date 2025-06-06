using EAP.Client.NonSecs.Message;
using EAP.Client.RabbitMq;
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
        private readonly RabbitMqService rabbitMqService;
        private readonly IConfiguration configuration;

        public PrimaryS5F1(RabbitMqService rabbitMqService, IConfiguration configuration)
        {
            this.rabbitMqService = rabbitMqService;
            this.configuration = configuration;
        }
        public async Task HandlePrimaryMessage(NonSecsMessageWrapper wrapper)
        {
            var s5f2 = new NonSecsMessage(5, 2);
            await wrapper.TryReplyAsync(s5f2);

            var s5f1 = JsonConvert.DeserializeObject<S5F1>(wrapper.PrimaryMessageString);

            var equipmentId = configuration.GetSection("Custom")["EquipmentId"];
            var equipmentType = configuration.GetSection("Custom")["EquipmentType"];

            var para = new Dictionary<string, object> {
                            { "AlarmEqp", equipmentId},
                            { "AlarmCode",s5f1.AlarmID},
                            { "AlarmText",s5f1.AlarmText},
                            { "AlarmSource", "EAP"},
                            { "AlarmTime",DateTime.Now},
                            { "AlarmSet",s5f1.AlarmSet =="Y"}
                        };
            var trans = new RabbitMqTransaction
            {
                TransactionName = "EquipmentAlarm",
                Parameters = para,
            };
            rabbitMqService.Produce("EAP.Services", trans);

        }
    }
}
