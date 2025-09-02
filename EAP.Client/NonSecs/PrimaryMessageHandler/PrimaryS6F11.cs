using EAP.Client.NonSecs.Message;
using EAP.Client.RabbitMq;
using log4net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EAP.Client.NonSecs.PrimaryMessageHandler
{
    public class PrimaryS6F11 : IPrimaryMessageHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        private readonly RabbitMqService rabbitMqService;
        private readonly IConfiguration configuration;

        public PrimaryS6F11(RabbitMqService rabbitMqService, IConfiguration configuration)
        {
            this.rabbitMqService = rabbitMqService;
            this.configuration = configuration;
        }
        public async Task HandlePrimaryMessage(NonSecsMessageWrapper wrapper)
        {
            try
            {
                var s6f12 = new NonSecsMessage(6, 12);
                await wrapper.TryReplyAsync(s6f12);

                var s6f11 = JsonConvert.DeserializeObject<S6F11>(wrapper.PrimaryMessageString);
                var vidDict = configuration.GetSection("NonSecs:VidDict").Get<Dictionary<string, string>>();
                var rptDict = configuration.GetSection("NonSecs:RptDict").Get<Dictionary<string, string>>();
                var equipmentId = configuration.GetSection("Custom")["EquipmentId"];
                var EventName = rptDict.TryGetValue(s6f11.EventID, out string name) ? name : "UnknownEvent";
                //var subEqDict = configuration.GetSection("NonSecs:SubEquipment").Get<Dictionary<string, string>>();

                string svidResult = string.Join(",", s6f11.Reports.Select(kvp => $"{(vidDict.TryGetValue(kvp.Key, out string paramName) ? paramName : kvp.Key)}={kvp.Value}"));


                UploadParameter(equipmentId, EventName, s6f11.EventID, svidResult);

                    

                
              
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex);
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
