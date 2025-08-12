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
    public class PrimaryS6F11 : IPrimaryMessageHandler
    {
        private readonly RabbitMqService rabbitMqService;
        private readonly IConfiguration configuration;

        public PrimaryS6F11(RabbitMqService rabbitMqService, IConfiguration configuration)
        {
            this.rabbitMqService = rabbitMqService;
            this.configuration = configuration;
        }
        public async Task HandlePrimaryMessage(NonSecsMessageWrapper wrapper)
        {
            var s6f12 = new NonSecsMessage(6, 12);
            await wrapper.TryReplyAsync(s6f12);

            var s6f11 = JsonConvert.DeserializeObject<S6F11>(wrapper.PrimaryMessageString);

            //TODO:按照EventID处理S6F11
            var vidDict = configuration.GetSection("NonSecs:VidDict").Get<Dictionary<string, string>>();
            var equipmentId = configuration.GetSection("Custom")["EquipmentId"];

            // 确保 Reports 不为 null
            if (s6f11.Reports != null)
            {
                var mappedReports = new Dictionary<string, string>();

                foreach (var kvp in s6f11.Reports)
                {
                    var svid = kvp.Key;
                    var value = kvp.Value;

                    // 通过映射找到对应的 Name
                    var name = vidDict.TryGetValue(svid, out var mappedName)
                               ? mappedName
                               : svid; // 映射不到就用原 SVID

                    // 调用你的处理方法
                    UploadParameter(equipmentId, name, svid, value);
                }

            }
        }

        void UploadParameter(string subEQID, string name, string svid, string value)
        {
            var para = new Dictionary<string, object>
             {
                 { "EQID", subEQID },
                 { "NAME", name },
                 { "SVID", svid },
                 { "Value", value },
                 { "UPDATETIME", DateTime.Now }
             };

            var paratrans = new RabbitMqTransaction
            {
                TransactionName = "EquipmentParams",
                EquipmentID = subEQID,
                Parameters = para
            };

            rabbitMqService.Produce("EAP.Services", paratrans);
        }


    }
}
