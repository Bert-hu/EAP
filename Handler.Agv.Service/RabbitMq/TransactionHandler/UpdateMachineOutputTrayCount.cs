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
        public UpdateMachineOutputTrayCount(ISqlSugarClient sqlSugarClient)
        {
            this.sqlSugarClient = sqlSugarClient;
        }

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            try
            {
                var machine = sqlSugarClient.Queryable<HandlerEquipmentStatus>().InSingle(trans.EquipmentID);
                if (machine != null)
                {
                    if (trans.Parameters?.ContainsKey("OutputTrayCount") ?? false)
                    {
                        var OutputTrayCount = int.Parse(trans.Parameters["OutputTrayCount"].ToString());
                        machine.OutputTrayNumber = OutputTrayCount;
                        machine.OutputTrayUpdateTime = DateTime.Now;
                        await sqlSugarClient.Updateable(machine)
                            .UpdateColumns(it => new { it.OutputTrayNumber, it.OutputTrayUpdateTime })
                            .ExecuteCommandAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.Message, ex);
            }
        }
    }
}
