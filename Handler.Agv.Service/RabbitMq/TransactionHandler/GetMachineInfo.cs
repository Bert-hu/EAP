using HandlerAgv.Service.Models.Database;
using log4net;
using Newtonsoft.Json.Linq;
using SqlSugar;

namespace HandlerAgv.Service.RabbitMq.TransactionHandler
{
    public class GetMachineInfo : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        private ISqlSugarClient sqlSugarClient;
        private readonly RabbitMqService rabbitMqService;
        public GetMachineInfo(ISqlSugarClient sqlSugarClient, RabbitMqService rabbitMqService)
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
                    repTrans.Parameters.Add("Result", true);
                    repTrans.Parameters.Add("AgvEnabled", machine.AgvEnabled);
                    repTrans.Parameters.Add("InputTrayCount", machine.InputTrayNumber);
                    repTrans.Parameters.Add("OutputTrayCount", machine.OutputTrayNumber);
                    repTrans.Parameters.Add("CurrentLot", machine.CurrentLot);
                    repTrans.Parameters.Add("GroupName", machine.GroupName);
                    repTrans.Parameters.Add("MaterialName", machine.MaterialName);

                    string currentTaskState = "未知状态";

                    if (!string.IsNullOrEmpty(machine.CurrentTaskId))
                    {
                        var task = sqlSugarClient.Queryable<HandlerAgvTask>()
                            .Where(t => t.ID == machine.CurrentTaskId)
                            .First();
                        if (task != null)
                        {
                            switch (task.Status)
                            {
                                case AgvTaskStatus.AgvRequested:
                                    currentTaskState = "AGV任务已请求";
                                    break;
                                case AgvTaskStatus.AgvArrived:
                                    currentTaskState = "AGV已到达";
                                    break;
                                case AgvTaskStatus.MachineReady:
                                    currentTaskState = "设备已锁定进出料";
                                    break;
                                case AgvTaskStatus.AgvRobotFinished:
                                    currentTaskState = "AGV手臂任务已完成";
                                    break;
                                default:
                                    currentTaskState = "未知状态";
                                    break;
                            }
                        }
                    }
                    else
                    {
                        currentTaskState = "无AGV任务";
                    }
                    repTrans.Parameters.Add("CurrentTaskState", currentTaskState);

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
                repTrans.Parameters.Add("Message", "Error occurred while retrieving machine info");
            }
            rabbitMqService.Produce(trans.ReplyChannel, repTrans);
        }
    }
}
