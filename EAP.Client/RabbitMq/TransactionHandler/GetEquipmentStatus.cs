using EAP.Client.Forms;
using log4net;
using Microsoft.Extensions.Configuration;

namespace EAP.Client.RabbitMq
{
    public class GetEquipmentStatus : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        private readonly IConfiguration configuration;
        private readonly RabbitMqService rabbitMq;



        public GetEquipmentStatus(IConfiguration configuration, RabbitMqService rabbitMq)
        {
            this.configuration = configuration;
            this.rabbitMq = rabbitMq;
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

                    string controlState = "Unknown";
                    string processState = "Unknown";

                    //commonLibrary.SecsConfigs.ProcessStateCodes.TryGetValue(processStateCode.ToString(), out processState);

                    reptrans?.Parameters.Add("Result", true);
                    reptrans?.Parameters.Add("ControlState", controlState);
                    reptrans?.Parameters.Add("ProcessState", processState);
                }
                catch (Exception ex)
                {
                    Status = "EAP Error";
                    reptrans?.Parameters.Add("Result", false);
                    reptrans?.Parameters.Add("Message", $"EAP Error {ex.Message}");
                    dbgLog.Error(ex.Message, ex);
                }
                reptrans?.Parameters.Add("Status", Status);
                if (reptrans != null) rabbitMq.Produce(trans.ReplyChannel, reptrans);
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
