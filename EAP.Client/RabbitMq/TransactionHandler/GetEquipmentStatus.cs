using EAP.Client.Forms;
using EAP.Client.NonSecs.Message;
using log4net;
using Microsoft.Extensions.Configuration;

namespace EAP.Client.RabbitMq
{
    public class GetEquipmentStatus : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        private readonly IConfiguration configuration;
        private readonly RabbitMqService rabbitMq;
        private readonly NonSecsService nonSecsService;



        public GetEquipmentStatus(IConfiguration configuration, RabbitMqService rabbitMq, NonSecsService nonSecsService)
        {
            this.configuration = configuration;
            this.rabbitMq = rabbitMq;
            this.nonSecsService = nonSecsService;
        }

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            try
            {
                var reptrans = trans?.GetReplyTransaction();
                string Status = "Offline";

                try
                {
                    //TODO get status svid


                    var paramsVid = configuration.GetSection("NonSecs:ParamsVid").Get<List<string>>();
                    var vidDict = configuration.GetSection("NonSecs:VidDict").Get<Dictionary<string, string>>();
                    if (nonSecsService.connectionState == NonSecsService.ConnectionState.Connected)
                    {
                        var s1f3 = new S1F3() { List = paramsVid };
                        var reply = await nonSecsService.SendMessage(s1f3);

                        var s1f4 = reply.SecondaryMessage as S1F4;

                        if (vidDict.ContainsValue("Status"))
                        {
                            var statusDict = configuration.GetSection("NonSecs:ProcessStateCodes").Get<Dictionary<string, string>>();
                            var statusVid = vidDict.FirstOrDefault(it => it.Value == "Status").Key;

                            var statusString = string.Empty;
                            s1f4?.List?.TryGetValue(statusVid, out statusString);

                            if (statusDict.ContainsKey(statusString))
                            {
                                Status = statusDict[statusString];
                            }
                            else
                            {
                                Status = "Unknown";
                            }
                        }
                        var equipmentId = configuration.GetSection("Custom")["EquipmentId"];
                        foreach (var item in s1f4?.List)
                        {
                            var name = string.Empty;
                            vidDict.TryGetValue(item.Key, out name);
                            if (!string.IsNullOrEmpty(name))
                            {
                                var para = new Dictionary<string, object> {
                                { "EQID",equipmentId },
                                { "NAME",name},
                                { "SVID",item.Key},
                                { "Value",item.Value},
                                { "UPDATETIME",DateTime.Now}
                            };
                                var paratrans = new RabbitMqTransaction
                                {
                                    TransactionName = "EquipmentParams",
                                    Parameters = para
                                };
                                rabbitMq.Produce("EAP.Services", paratrans);
                            }
                        }
                    }

                    reptrans?.Parameters?.Add("Result", true);
                }
                catch (Exception ex)
                {
                    Status = "EAP Error";
                    reptrans?.Parameters.Add("Result", false);
                    reptrans?.Parameters.Add("Message", $"EAP Error {ex.Message}");
                    dbgLog.Error(ex.Message, ex);
                }
                reptrans?.Parameters?.Add("Status", Status);
                if (reptrans != null && reptrans.NeedReply) rabbitMq.Produce(trans!.ReplyChannel, reptrans);
                UpdateEquipmentStatus(Status);
                MainForm.Instance?.UpdateState(Status);
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex);
            }
        }

        public void UpdateEquipmentStatus(string equipmentState)
        {
            var para = new Dictionary<string, object> {
                        { "EQID",configuration.GetSection("Custom")["EquipmentId"] },
                        { "DateTime",DateTime.Now},
                        { "EQType",configuration.GetSection("Custom")["EquipmentType"] },
                        { "Status",equipmentState}
                    };
            RabbitMqTransaction trans = new RabbitMqTransaction
            {
                TransactionName = "EquipmentStatus",
                Parameters = para
            };
            rabbitMq.Produce("EAP.Services", trans);
        }
    }
}
