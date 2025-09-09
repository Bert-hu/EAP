using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.Services;
using log4net;
using Newtonsoft.Json.Linq;
using SqlSugar;

namespace HandlerAgv.Service.RabbitMq.TransactionHandler
{
    public class MachineEventTrigger : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        private ISqlSugarClient sqlSugarClient;
        private readonly RabbitMqService rabbitMqService;
        public MachineEventTrigger(ISqlSugarClient sqlSugarClient, RabbitMqService rabbitMqService)
        {
            this.sqlSugarClient = sqlSugarClient;
            this.rabbitMqService = rabbitMqService;
        }

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            try
            {
                string eventName = trans.Parameters["EventName"].ToString();
                string processState = trans.Parameters["ProcessState"].ToString();
                string processStateCode = trans.Parameters["ProcessStateCode"].ToString();
                string recipeName = trans.Parameters["RecipeName"].ToString();
                string alarmList = trans.Parameters["AlarmList"].ToString();
                bool lockState = bool.Parse(trans.Parameters["LockState"].ToString() ?? "False");
                bool cleanOut = bool.Parse(trans.Parameters["CleanOut"].ToString() ?? "False");
                bool auto1Full = bool.Parse(trans.Parameters["Auto1Full"].ToString() ?? "False");
                DateTime eventTime = DateTime.Now;//改为服务器时间，防止机器时间不准

                var eventHist = new HandlerEventHist
                {
                    EquipmentId = trans.EquipmentID,
                    EventName = eventName,
                    ProcessState = processState,
                    ProcessStateCode = processStateCode,
                    RecipeName = recipeName,
                    AlarmList = alarmList,
                    LockState = lockState,
                    CleanOut = cleanOut,
                    Auto1Full = auto1Full,
                };
                sqlSugarClient.Insertable<HandlerEventHist>(eventHist).ExecuteCommand();

                var machine = sqlSugarClient.Queryable<HandlerEquipmentStatus>().InSingle(trans.EquipmentID);
                if (machine != null)
                {
                    machine.ProcessState = processState;
                    machine.ProcessStateCode = processStateCode;
                    machine.RecipeName = recipeName;
                    machine.AlarmList = alarmList;
                    machine.CleanOutState = cleanOut;
                    machine.Auto1FullState = auto1Full;

                    machine.UpdateTime = eventTime;

                    sqlSugarClient.Updateable(machine)
                        .UpdateColumns(it => new { it.ProcessState, it.ProcessStateCode, it.RecipeName, it.UpdateTime })
                        .ExecuteCommand();
                }
                switch (eventName)
                {
                    case "LoadLastPanel":
                        machine.LoaderEmpty = true;
                        sqlSugarClient.Updateable(machine).UpdateColumns(it => new { it.LoaderEmpty }).ExecuteCommand();
                        EapClientService service = new EapClientService(sqlSugarClient, rabbitMqService);
                        if (machine.IsValiad && machine.AgvEnabled)
                        {
                            service.MachineAgvLock(machine.Id);
                            service.UpdateClientInfo(trans.EquipmentID, "最后一盘已下沉,检测AGV模式已开，提前锁定机器");
                            dbgLog.Info($"{machine.Id} 最后一盘已下沉,锁定成功");
                        }

                        break;
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
