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
                //string Status = "Offline";

                try
                {
                    //TODO get status svid


                    var paramsVid = configuration.GetSection("NonSecs:ParamsVid").Get<List<string>>();
                    var vidDict = configuration.GetSection("NonSecs:VidDict").Get<Dictionary<string, string>>();

                    // 加载Status映射字典
                    var statusDict = configuration.GetSection("NonSecs:ProcessStateCodes").Get<Dictionary<string, string>>();

                    // 存每个子设备状态
                    var statusMap = new Dictionary<string, string>(); 

                    if (nonSecsService.connectionState == NonSecsService.ConnectionState.Connected)
                    {
                        var s1f3 = new S1F3() { List = paramsVid };
                        var reply = await nonSecsService.SendMessage(s1f3);

                        var s1f4 = reply.SecondaryMessage as S1F4;

                        // 遍历所有包含Status的字段
                        foreach (var kvp in vidDict.Where(it => it.Value == "Status"))
                        {
                            var statusVid = kvp.Key;       // VID编号
                            var subEQIndex = statusVid[..1];  // 取VID第一位数字，区分子设备

                            // 获取对应subEQID（示例映射关系，实际可用配置或字典控制）
                            string subEQID = subEQIndex switch
                            {
                                "1" => "EQASM00010",
                                "2" => "EQASM00011",
                                "3" => "EQASM00012",
                                "4" => "EQTBU00007",
                                "5" => "EQASM00013",
                                "6" => "EQASM00014",
                                "7" => "EQTBU00008",
                                _ => "Unknown",
                                //Load, Unload 尚无
                            };
                            string statusString = string.Empty;
                            // 提取状态值
                            if (s1f4?.List?.TryGetValue(statusVid, out statusString) == true)
                            {
                                var mappedStatus = statusDict.ContainsKey(statusString) ? statusDict[statusString] : "Unknown";

                                // 存入字典
                                statusMap[subEQID] = mappedStatus;
                            }
                            else
                            {
                                // 没取到值时
                                statusMap[subEQID] = "Unknown";
                            }
                        }
                    }
                    // 针对每个子设备独立Produce
                    foreach (var kvp in statusMap)
                    {
                        string subEQID = kvp.Key;
                        string Status = kvp.Value;

                        try
                        {
                            reptrans?.Parameters.Add("Status", Status);
                            reptrans.EquipmentID = subEQID;
                            //reptrans?.Parameters.Add("EquipmentID", subEQID);

                            if (reptrans != null && reptrans.NeedReply)
                                rabbitMq.Produce(trans!.ReplyChannel, reptrans);

                            UpdateEquipmentStatus(Status, subEQID); // 传入对应子设备
                            MainForm.Instance?.UpdateState(Status, subEQID);
                        }
                        catch (Exception ex)
                        {
                            reptrans?.Parameters.Add("Result", false);
                            reptrans?.Parameters.Add("Message", $"EAP Error {ex.Message}");
                            dbgLog.Error(ex.Message, ex);

                            if (reptrans != null && reptrans.NeedReply)
                                rabbitMq.Produce(trans!.ReplyChannel, reptrans);
                        }
                    }
                    #region old method
                    //    if (vidDict.ContainsValue("Status"))
                    //    {

                    //        var statusVid = vidDict.FirstOrDefault(it => it.Value == "Status").Key;

                    //        var statusString = string.Empty;
                    //        s1f4?.List?.TryGetValue(statusVid, out statusString);

                    //        if (statusDict.ContainsKey(statusString))
                    //        {
                    //            Status = statusDict[statusString];
                    //        }
                    //        else
                    //        {
                    //            Status = "Unknown";
                    //        }
                    //    }
                    //    var equipmentId = configuration.GetSection("Custom")["EquipmentId"];
                    //    foreach (var item in s1f4?.List)
                    //    {
                    //var name = string.Empty;
                    //vidDict.TryGetValue(item.Key, out name);
                    //if (!string.IsNullOrEmpty(name))
                    //{
                    //    var para = new Dictionary<string, object> {
                    //            { "EQID",equipmentId },
                    //            { "NAME",name},
                    //            { "SVID",item.Key},
                    //            { "Value",item.Value},
                    //            { "UPDATETIME",DateTime.Now}
                    //        };
                    //    var paratrans = new RabbitMqTransaction
                    //    {
                    //        TransactionName = "EquipmentParams",
                    //        Parameters = para
                    //    };
                    //    rabbitMq.Produce("EAP.Services", paratrans);
                    //}
                    //    }
                    //}

                    //reptrans?.Parameters?.Add("Result", true);
                    #endregion
                }
                catch (Exception ex)
                {
                    //Status = "EAP Error";
                    //reptrans?.Parameters.Add("Result", false);
                    //reptrans?.Parameters.Add("Message", $"EAP Error {ex.Message}");
                    dbgLog.Error(ex.Message, ex);
                }
                //reptrans?.Parameters?.Add("Status", Status);
                //if (reptrans != null && reptrans.NeedReply) rabbitMq.Produce(trans!.ReplyChannel, reptrans);
                //UpdateEquipmentStatus(Status);
                //MainForm.Instance?.UpdateState(Status);
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
        public void UpdateEquipmentStatus(string equipmentState, string subEQID)
        {
            var para = new Dictionary<string, object> {
                        { "EQID",subEQID },
                        { "DateTime",DateTime.Now},
                        { "EQType",configuration.GetSection("Custom")["EquipmentType"] },
                        { "Status",equipmentState}
                    };
            RabbitMqTransaction trans = new RabbitMqTransaction
            {
                TransactionName = "EquipmentStatus",
                EquipmentID = subEQID,
                Parameters = para
            };
            rabbitMq.Produce("EAP.Services", trans);
        }
    }
}
