using HandlerAgv.Service.Models.Database;
using log4net;
using Newtonsoft.Json.Linq;
using SqlSugar;

namespace HandlerAgv.Service.RabbitMq.TransactionHandler
{
    public class MachineEventTrigger : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        private ISqlSugarClient sqlSugarClient;
        public MachineEventTrigger(ISqlSugarClient sqlSugarClient)
        {
            this.sqlSugarClient = sqlSugarClient;
        }

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            try
            {
                string eventName = trans.Parameters["EventName"].ToString();
                string processState = trans.Parameters["ProcessState"].ToString();
                string processStateCode = trans.Parameters["ProcessStateCode"].ToString();
                string recipeName = trans.Parameters["RecipeName"].ToString();
                DateTime eventTime = DateTime.Now;//改为服务器时间，防止机器时间不准

                var eventHist = new HandlerEventHist
                {
                    EquipmentId = trans.EquipmentID,
                    EventName = eventName,
                    ProcessState = processState,
                    ProcessStateCode = processStateCode,
                    RecipeName = recipeName,
                };
                sqlSugarClient.Insertable<HandlerEventHist>(eventHist).ExecuteCommand();

                var machine = sqlSugarClient.Queryable<HandlerEquipmentStatus>().InSingle(trans.EquipmentID);
                if (machine != null)
                {
                    machine.ProcessState = processState;
                    machine.ProcessStateCode = processStateCode;
                    machine.RecipeName = recipeName;
                    machine.UpdateTime = eventTime;
                    await sqlSugarClient.Updateable(machine)
                        .UpdateColumns(it => new { it.ProcessState, it.ProcessStateCode, it.RecipeName, it.UpdateTime })
                        .ExecuteCommandAsync();
                }
                switch (eventName)
                {               
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.Message, ex);
            }
        }
    }
}
