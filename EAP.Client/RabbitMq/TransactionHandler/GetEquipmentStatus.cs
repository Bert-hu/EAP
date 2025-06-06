using log4net;
using Secs4Net;
using EAP.Client.Forms;
using EAP.Client.Secs;
using static Secs4Net.Item;

namespace EAP.Client.RabbitMq
{
    public class GetEquipmentStatus :  ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        private readonly RabbitMqService rabbitMq;
        private readonly ISecsGem secsGem;
        private readonly CommonLibrary commonLibrary;

        public GetEquipmentStatus(RabbitMqService rabbitMq, ISecsGem secsGem, CommonLibrary commonLibrary)
        {
            this.rabbitMq = rabbitMq;
            this.secsGem = secsGem;
            this.commonLibrary = commonLibrary;
        }

        public  async Task HandleTransaction(RabbitMqTransaction trans)
        {
            try
            {
                var reptrans = trans?.GetReplyTransaction();
                string Status = "Offline";
                var secs = secsGem as SecsGem;
                if (secs._hsmsConnector != null && secs._hsmsConnector.State == ConnectionState.Selected)
                {
                    try
                    {
                        var controlStateVID = commonLibrary.GetGemSvid("ControlState");
                        var processStateVID = commonLibrary.GetGemSvid("ProcessState");
                        SecsMessage s1f3 = new(1, 3, true)
                        {
                            SecsItem = L(
                                U4((uint)controlStateVID.ID),
                                U4((uint)processStateVID.ID)
                                )
                        };
                        var s1f4 = await secsGem.SendAsync(s1f3);
                        var controlStateCode = s1f4.SecsItem[0].FirstValue<byte>();
                        var processStateCode = s1f4.SecsItem[1].FirstValue<byte>();
                        string controlState = "Unknown";
                        string processState = "Unknown";
                        switch (controlStateCode)
                        {
                            case 1:
                            case 2:
                            case 3:
                                controlState = "Offline";
                                break;
                            case 4:
                                controlState = "OnlineLocal";
                                break;
                            case 5:
                                controlState = "OnlineRemote";
                                break;
                            default:
                                controlState = "Unknown";
                                break;
                        }
                        commonLibrary.SecsConfigs.ProcessStateCodes.TryGetValue(processStateCode.ToString(), out processState);

                        if (controlState == "OnlineLocal" || controlState == "OnlineRemote") Status = processState ?? "Unknown";
                        reptrans?.Parameters.Add("Result", true);
                        reptrans?.Parameters.Add("ControlState", controlState);
                        reptrans?.Parameters.Add("ProcessState", processState);
                    }
                    catch (SecsException ex)
                    {
                        Status = "Offline";
                        reptrans?.Parameters.Add("Result", true);
                        reptrans?.Parameters.Add("Message", $"Eqp Offline");
                        dbgLog.Error(ex.Message, ex);
                    }
                    catch (Exception ex)
                    {
                        Status = "EAP Error";
                        reptrans?.Parameters.Add("Result", false);
                        reptrans?.Parameters.Add("Message", $"EAP Error {ex.Message}");
                        dbgLog.Error(ex.Message, ex);
                    }
                }
                else
                {
                    //if (secs._hsmsConnector.State != ConnectionState.Selected) hsmsConnection.Reconnect();
                }
                reptrans?.Parameters.Add("Status", Status);
                if (reptrans != null) rabbitMq.Produce(trans.ReplyChannel, reptrans);
                UpdateEquipmentStatus(Status);
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
                        { "EQID",commonLibrary.CustomSettings["EquipmentId"] },
                        { "DateTime",DateTime.Now},
                        { "EQType",commonLibrary.CustomSettings["EquipmentType"] },
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
