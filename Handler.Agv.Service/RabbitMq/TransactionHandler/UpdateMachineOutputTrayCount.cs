using HandlerAgv.Service.Models.Database;
using log4net;
using Newtonsoft.Json.Linq;
using SqlSugar;

namespace HandlerAgv.Service.RabbitMq.TransactionHandler
{
    public class UpdateMachineOutputTrayCount : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        private ISqlSugarClient sqlSugarClient;
        private readonly RabbitMqService rabbitMqService;
        public UpdateMachineOutputTrayCount(ISqlSugarClient sqlSugarClient, RabbitMqService rabbitMqService)
        {
            this.sqlSugarClient = sqlSugarClient;
            this.rabbitMqService = rabbitMqService;
        }

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var repTrans = trans.GetReplyTransaction();
            try
            {
                var machine = sqlSugarClient.Queryable<HandlerEquipmentStatus>().InSingle(trans.EquipmentID);
                if (machine != null)
                {
                    var OutputTrayCount = int.Parse(trans.Parameters["OutputTrayCount"].ToString());
                    machine.OutputTrayNumber = OutputTrayCount;
                    machine.OutputTrayUpdateTime = DateTime.Now;
                    await sqlSugarClient.Updateable(machine)
                        .UpdateColumns(it => new { it.OutputTrayNumber, it.OutputTrayUpdateTime })
                        .ExecuteCommandAsync();
                    repTrans.Parameters.Add("Result", true);
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
            rabbitMqService.Produce(trans.ReplyChannel, repTrans);
        }
    }
}
