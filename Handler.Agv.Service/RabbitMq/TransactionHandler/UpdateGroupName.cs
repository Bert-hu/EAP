using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.Services;
using log4net;
using SqlSugar;

namespace HandlerAgv.Service.RabbitMq.TransactionHandler
{
    public class UpdateGroupName : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        private ISqlSugarClient sqlSugarClient;
        private readonly RabbitMqService rabbitMqService;

        public UpdateGroupName(ISqlSugarClient sqlSugarClient, RabbitMqService rabbitMqService)
        {
            this.sqlSugarClient = sqlSugarClient;
            this.rabbitMqService = rabbitMqService;
        }

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var repTrans = trans.GetReplyTransaction();
            try
            {
                // 查询设备信息
                var machine = sqlSugarClient.Queryable<HandlerEquipmentStatus>()
                    .InSingle(trans.EquipmentID);

                if (machine != null)
                {
                    // 获取并更新组名称
                    var groupName = trans.Parameters["GroupName"].ToString();
                    machine.GroupName = groupName;

                    await sqlSugarClient.Updateable(machine)
                        .UpdateColumns(it => new { it.GroupName })
                        .ExecuteCommandAsync();

                    repTrans.Parameters.Add("Result", true);
                    dbgLog.Info($"{machine.Id} GroupName更新: {groupName}");

                    // 同步更新客户端信息
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
                repTrans.Parameters.Add("Message", "Error occurred while updating group name");
            }

            // 发送回复消息
            rabbitMqService.Produce(trans.ReplyChannel, repTrans);
        }
    }
}
