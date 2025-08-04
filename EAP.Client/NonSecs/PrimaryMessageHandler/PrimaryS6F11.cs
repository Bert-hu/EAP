using EAP.Client.NonSecs.Message;
using EAP.Client.RabbitMq;
using log4net;
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
using System.Windows.Media.Media3D;
using System.Xml.Linq;

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
            var s6f12 = new NonSecsMessage(6, 12);
            await wrapper.TryReplyAsync(s6f12);

            var s6f11 = JsonConvert.DeserializeObject<S6F11>(wrapper.PrimaryMessageString);

            if (string.IsNullOrEmpty(s6f11?.EventID))
            {
                dbgLog.Warn($"[HandlePrimaryMessage] - S6F11 - EventID is null OR empty.");
                return;
            }

            var handlers = new Dictionary<string, Action<S6F11>>
            {
                { "6001", HandleReelIntegratedFromEvent },
                //{ "6002", HandleRIDMFromEvent },
                { "6003", HandleTotalRIDMFromEvent }
            };

            if (handlers.TryGetValue(s6f11.EventID, out var handler))
            {
                handler(s6f11);
            }
            else
            {
                // 处理未识别事件
                dbgLog.Warn($"[HandlePrimaryMessage] - EventNotFound - {s6f11.EventID}.");
                return;
            }

          
        }

        void HandleReelIntegratedFromEvent(S6F11 message)
        {
            var vidDict = configuration.GetSection("NonSecs:VidDict").Get<Dictionary<string, string>>();

            var equipmentId = configuration.GetSection("Custom")["EquipmentId"];

            string recipeName = GetReportValue(message, "1003");
            string workOrder = GetReportValue(message, "1004");
            string material = GetReportValue(message, "1005");

            string integratedKey = new[] { "1007", "1008", "1009" }.FirstOrDefault(k => message.Reports.ContainsKey(k)) ?? "1009";
            string integratedSum = GetReportValue(message, integratedKey);

            UploadParameter(equipmentId, GetVidName(vidDict, "1003"), "1003", recipeName);
            UploadParameter(equipmentId, GetVidName(vidDict, "1004"), "1004", workOrder);
            UploadParameter(equipmentId, GetVidName(vidDict, "1005"), "1005", material);
            UploadParameter(equipmentId, GetVidName(vidDict, integratedKey), integratedKey, integratedSum);

           
        }
        void HandleRIDMFromEvent(S6F11 message) {
            var vidDict = configuration.GetSection("NonSecs:VidDict").Get<Dictionary<string, string>>();

            var equipmentId = configuration.GetSection("Custom")["EquipmentId"];

            string workOrder = GetReportValue(message, "1004");
            string material = GetReportValue(message, "1005");

            var para = new Dictionary<string, object> {
                //{ "eventID", eventID},
                { "EQID", equipmentId},
                { "workOrder", workOrder},
                { "material", material}

            };
            UploadReport(equipmentId, "GetRIDM", para);
        }
        void HandleTotalRIDMFromEvent(S6F11 message)
        {
            var vidDict = configuration.GetSection("NonSecs:VidDict").Get<Dictionary<string, string>>();

            var equipmentId = configuration.GetSection("Custom")["EquipmentId"];

            string recipeName = GetReportValue(message, "1003");
            string workOrder = GetReportValue(message, "1004");

            var para = new Dictionary<string, object> {
                //{ "eventID", eventID},
                { "EQID", equipmentId},
                { "workOrder", workOrder},
                { "recipeName", recipeName}

            };
            UploadReport(equipmentId, "GetTotalRIDM", para);


        }

        string GetReportValue(S6F11 message, string key) =>
            message.Reports.TryGetValue(key, out var value) ? value : string.Empty;

        string GetVidName(Dictionary<string, string> vidDict, string key) =>
            vidDict.TryGetValue(key, out var name) ? name : $"Unknown_{key}";

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

        void UploadReport(string EQID,string transName, Dictionary<string, object> para)
        {
            RabbitMqTransaction trans = new RabbitMqTransaction
            {
                EquipmentID = EQID,
                TransactionName = transName,
                Parameters = para
            };
            rabbitMqService.Produce("EAP.RIDMTest", trans);
        }
    }
}
