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
        public UpdateMachineInputTrayCount(ISqlSugarClient sqlSugarClient)
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
                    if (trans.Parameters?.ContainsKey("InputTrayCount") ?? false)
                    {
                        var inputTrayCount = int.Parse(trans.Parameters["InputTrayCount"].ToString());
                        machine.InputTrayNumber = inputTrayCount;
                        machine.InputTrayUpdateTime = DateTime.Now;
                        await sqlSugarClient.Updateable(machine)
                            .UpdateColumns(it => new { it.InputTrayNumber, it.InputTrayUpdateTime })
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
