using HandlerAgv.Service.Models.Database;
using log4net;
using Newtonsoft.Json.Linq;
using SqlSugar;

namespace HandlerAgv.Service.RabbitMq.TransactionHandler
{
    public class GetMachineInfo : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        private ISqlSugarClient sqlSugarClient;
        public GetMachineInfo(ISqlSugarClient sqlSugarClient)
        {
            this.sqlSugarClient = sqlSugarClient;
        }

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            try
            {
                var repTrans = trans.GetReplyTransaction();
                var machine = sqlSugarClient.Queryable<HandlerEquipmentStatus>().InSingle(trans.EquipmentID);
                if (machine != null)
                {
                    repTrans.Parameters.Add("Result", true);
                    repTrans.Parameters.Add("AgvEnabled", machine.AgvEnabled);
                    repTrans.Parameters.Add("InputTrayCount", machine.InputTrayNumber);
                    repTrans.Parameters.Add("OutputTrayCount", machine.OutputTrayNumber);
                }
                else
                {                   
                    repTrans.Parameters.Add("Result", false);
                    repTrans.Parameters.Add("Message", "Machine not found");
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.Message, ex);
            }
        }
    }
}
