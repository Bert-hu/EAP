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
            var vidDict = configuration.GetSection("NonSecs:VidDict").Get<Dictionary<string, string>>();

            foreach (var item in s6f11.Reports) // 遍历参数
            {
                if (!vidDict.TryGetValue(item.Key, out var name) || string.IsNullOrEmpty(name))
                    continue;

                string vidStr = item.Key;
                string subEQID = "Unknown";
                string reportId = s6f11.EventID;

                // 判断ReportID
                if (reportId.Length == 2)
                {
                    // 两位数ReportID，取第一位
                    string index = reportId.Substring(0, 1);
                    subEQID = index switch
                    {
                        "1" => "EQPPR00010",
                        "2" => "EQCAS00001",
                        "3" => "EQPRE00030",
                        "4" => "EQCIS00002",
                        "5" => "EQPPR00011",
                        _ => "Unknown",
                        //Load, Unload 尚无
                    };

                    // by subEQID上传
                    UploadParameter(subEQID, name, vidStr, item.Value);
                }
                else if (reportId.Length == 1)
                {
                    // 一位数ReportID，检查VID
                    if (int.TryParse(vidStr, out int vidInt))
                    {
                        if (vidInt < 100)
                        {
                            // 每台设备都上传
                            foreach (var subIndex in new[] { "EQPPR00010", "EQCAS00001", "EQPRE00030", "EQCIS00002", "EQPPR00011" })
                            {
                                UploadParameter(subIndex, name, vidStr, item.Value);
                            }
                        }
                        else
                        {
                            // 根据VID第一位
                            string index = vidStr.Substring(0, 1);
                            subEQID = index switch
                            {
                                "1" => "EQPPR00010",
                                "2" => "EQCAS00001",
                                "3" => "EQPRE00030",
                                "4" => "EQCIS00002",
                                "5" => "EQPPR00011",
                                _ => "Unknown",
                            };

                            UploadParameter(subEQID, name, vidStr, item.Value);
                        }
                    }
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
