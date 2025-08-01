using EAP.Client.NonSecs.Message;
using EAP.Client.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
            if (!string.IsNullOrEmpty(s6f11?.EventID))
            {
                var eventId = s6f11?.EventID;
                switch (eventId)
                {
                    case "6001":
                        //HandleEqpStatusFromEvent(recMsg);
                        HandleReelIntegratedFromEvent(s6f11);
                        break;
                    case "6002":

                        //HandleRIDMFromEvent(recMsg);
                        break;
                    case "6003":

                        //HandleRIDMFromEvent(recMsg);
                        break;
                    default:
                        break;
                }
            }
        }

        private static void HandleReelIntegratedFromEvent(S6F11 message)
        {
           
            var eventID = message.EventID;
            var recipeName = message.Reports["1003"];
            var workOrder = message.Reports["1004"];
            var material = message.Reports["1005"];
            var integratedClass = string.Empty;
            var integratedSum = string.Empty;

            if (message.Reports.ContainsKey("1007"))
            {
                integratedClass = "1007";


            }
            else if (message.Reports.ContainsKey("1008"))
            {
                integratedClass = "1008";
            }
            else
            {
                integratedClass = "1009";
            }

            integratedSum = message.Reports[integratedClass];

            //HandleEqpParams("1003", recipeName);
            //HandleEqpParams("1004", workOrder);
            //HandleEqpParams("1005", material);

            //HandleEqpParams(integratedClass, integratedSum);

            //var para = new Dictionary<string, object> {
            //    { "eventID", eventID},
            //    { "recipeName", recipeName},
            //    { "workOrder", workOrder},
            //    { "material", material},
            //    { "integratedClass", integratedClass}, //几合几
            //    { "integratedSum", integratedSum}, //合了几盘

            //};

            //RabbitMqTransaction trans = new RabbitMqTransaction
            //{
            //    TransactionName = "EquipmentEvent",
            //    Parameters = para
            //};
            //sendToEapService("EAP.Services", trans);
        }
    }
}
