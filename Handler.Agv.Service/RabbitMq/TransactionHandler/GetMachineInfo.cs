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
                    repTrans.Parameters.Add("LoaderEmpty", machine.LoaderEmpty);

                    string currentTaskState = "未知状态";
                    string taskRequestTime = "无";

                    if (!string.IsNullOrEmpty(machine.CurrentTaskId))
                    {
                        var task = sqlSugarClient.Queryable<HandlerAgvTask>()
                            .Where(t => t.ID == machine.CurrentTaskId)
                            .First();

                        if (task != null)
                        {
                            taskRequestTime = ((DateTime)task.AgvRequestTime).ToString("M-d HH:mm");
                            string taskType = task.Type.ToString();

                            switch (task.Status)
                            {
                                case AgvTaskStatus.AgvRequested:
                                    currentTaskState = taskType + " 任务已请求";
                                    break;
                                case AgvTaskStatus.AgvArrived:
                                    currentTaskState = taskType + " AGV已到达";
                                    break;
                                case AgvTaskStatus.MachineReady:
                                    currentTaskState = taskType + " 设备已锁定";
                                    break;
                                case AgvTaskStatus.AgvRobotFinished:
                                    currentTaskState = taskType + " 已完成";
                                    break;
                                default:
                                    currentTaskState = taskType + " 未知状态";
                                    break;
                            }
                        }
                    }
                    else
                    {
                        currentTaskState = "无AGV任务";
                    }

                    repTrans.Parameters.Add("CurrentTaskState", currentTaskState);
                    repTrans.Parameters.Add("CurrentTaskRequestTime", taskRequestTime);

                    var agvInventory = "未知";
                    var stockInventory = "未知";
                    var inventory = sqlSugarClient.Queryable<HandlerInventory>()
                        .Where(t => t.MaterialName == machine.MaterialName && t.GroupName == machine.GroupName)
                        .First();

                    if (inventory != null)
                    {
                        agvInventory = inventory.AgvQuantity == -1 ? "未知" : inventory.AgvQuantity.ToString();
                        stockInventory = (inventory.Stocker1Quantity + inventory.Stocker2Quantity).ToString();
                    }

                    repTrans.Parameters.Add("AgvInventory", agvInventory);
                    repTrans.Parameters.Add("StockInventory", stockInventory);
                }
                else
                {
                    repTrans.Parameters.Add("Result", false);
                    repTrans.Parameters.Add("Message", "Machine not found");
                    dbgLog.Warn($"EAP Client not found for Equipment ID: {trans.EquipmentID}");
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error($"Error updating EAP Client Info for Equipment ID: {trans.EquipmentID}", ex);
                repTrans.Parameters.Add("Result", false);
                repTrans.Parameters.Add("Message", "Error occurred while retrieving machine info");
            }
            rabbitMqService.Produce(trans.ReplyChannel, repTrans);
        }
    }
}
