using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.Services;
using log4net;
using Newtonsoft.Json.Linq;
using SqlSugar;

namespace HandlerAgv.Service.RabbitMq.TransactionHandler
{
    public class UpdateAgvEnabled : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        private ISqlSugarClient sqlSugarClient;
        private readonly RabbitMqService rabbitMqService;
        public UpdateAgvEnabled(ISqlSugarClient sqlSugarClient, RabbitMqService rabbitMqService)
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
                if (machine == null)
                {
                    repTrans.Parameters.Add("Result", false);
                    repTrans.Parameters.Add("Message", "Machine not found");
                }
                else
                {
                    machine.AgvEnabled = bool.Parse(trans.Parameters["AgvEnabled"].ToString());
                    sqlSugarClient.Updateable(machine)
                        .UpdateColumns(it => new { it.AgvEnabled })
                        .ExecuteCommand();
                    repTrans.Parameters.Add("Result", true);
                    dbgLog.Debug($"AGV功能状态切换 {machine.Id}: {machine.AgvEnabled}");

                    EapClientService eapClient = new EapClientService(sqlSugarClient, rabbitMqService);
                    eapClient.UpdateClientInfo(trans.EquipmentID);
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.Message, ex);
                repTrans.Parameters.Add("Result", false);
                repTrans.Parameters.Add("Message", "Error occurred while updating AGV enabled status");
            }
            rabbitMqService.Produce(trans.ReplyChannel, repTrans);
        }
    }
}
