using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.Services;
using log4net;
using Newtonsoft.Json.Linq;
using SqlSugar;

namespace HandlerAgv.Service.RabbitMq.TransactionHandler
{
    public class LoaderEmpty : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        private ISqlSugarClient sqlSugarClient;
        private RabbitMqService rabbitMqService;
        public LoaderEmpty(ISqlSugarClient sqlSugarClient, RabbitMqService rabbitMqService)
        {
            this.sqlSugarClient = sqlSugarClient;
            this.rabbitMqService = rabbitMqService;
        }

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            try
            {
                var machine = sqlSugarClient.Queryable<HandlerEquipmentStatus>().InSingle(trans.EquipmentID);
                if (machine != null)
                {
                    machine.LoaderEmpty = true;
                    machine.InputTrayNumber = 0;
                    machine.InputTrayUpdateTime = DateTime.Now;
                    sqlSugarClient.Updateable(machine).UpdateColumns(it => new { it.LoaderEmpty, it.InputTrayNumber, it.InputTrayUpdateTime }).ExecuteCommand();
                    EapClientService service = new EapClientService(sqlSugarClient,rabbitMqService);
                    service.UpdateClientInfo(trans.EquipmentID, "更新LoaderEmpty状态成功");
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.Message, ex);
            }
        }
    }
}
