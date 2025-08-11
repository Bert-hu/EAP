using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.Services;
using log4net;
using Newtonsoft.Json.Linq;
using SqlSugar;

namespace HandlerAgv.Service.RabbitMq.TransactionHandler
{
    public class UpdateCurrentLot : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        private ISqlSugarClient sqlSugarClient;
        private readonly RabbitMqService rabbitMqService;
        public UpdateCurrentLot(ISqlSugarClient sqlSugarClient, RabbitMqService rabbitMqService)
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
                    var currentLot = trans.Parameters["CurrentLot"].ToString();
                    machine.CurrentLot = currentLot;
                    await sqlSugarClient.Updateable(machine)
                        .UpdateColumns(it => new { it.CurrentLot })
                        .ExecuteCommandAsync();
                    repTrans.Parameters.Add("Result", true);
                    dbgLog.Info($"{machine.Id} CurrentLot更新: {currentLot}");

                    EapClientService eapClient = new EapClientService(sqlSugarClient, rabbitMqService);
                    eapClient.UpdateClientInfo(trans.EquipmentID);
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
