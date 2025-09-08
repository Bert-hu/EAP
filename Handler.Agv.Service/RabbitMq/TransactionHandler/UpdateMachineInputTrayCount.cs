using HandlerAgv.Service.Models.Database;
using log4net;
using Newtonsoft.Json.Linq;
using SqlSugar;

namespace HandlerAgv.Service.RabbitMq.TransactionHandler
{
    public class UpdateMachineInputTrayCount : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        private ISqlSugarClient sqlSugarClient;
        private readonly RabbitMqService rabbitMqService;
        public UpdateMachineInputTrayCount(ISqlSugarClient sqlSugarClient, RabbitMqService rabbitMqService)
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
                    var inputTrayCount = int.Parse(trans.Parameters["InputTrayCount"].ToString());
                    machine.InputTrayNumber = inputTrayCount;
                    machine.InputTrayUpdateTime = DateTime.Now;
                    if (inputTrayCount > 0)
                    {
                        machine.LoaderEmpty = false;
                    }
                    await sqlSugarClient.Updateable(machine)
                        .UpdateColumns(it => new { it.InputTrayNumber, it.InputTrayUpdateTime,it.LoaderEmpty })
                        .ExecuteCommandAsync();
                    repTrans.Parameters.Add("Result", true);
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
                repTrans.Parameters.Add("Result", false);
                repTrans.Parameters.Add("Message", "Error occurred while updating input tray count");
            }
            rabbitMqService.Produce(trans.ReplyChannel, repTrans);
        }
    }
}
