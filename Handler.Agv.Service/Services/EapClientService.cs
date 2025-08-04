using AutoMapper;
using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.RabbitMq;
using log4net;
using SqlSugar;

namespace HandlerAgv.Service.Services
{
    public class EapClientService
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        private readonly ISqlSugarClient sqlSugarClient;
        private readonly RabbitMqService rabbitMqService;


        public EapClientService(ISqlSugarClient sqlSugarClient, RabbitMqService rabbitMqService)
        {
            this.sqlSugarClient = sqlSugarClient;
            this.rabbitMqService = rabbitMqService;
        }

        public void UpdateClientInfo(string equipmentId)
        {
            try
            {
                var equipment = sqlSugarClient.Queryable<HandlerEquipmentStatus>()
                    .Where(e => e.Id == equipmentId)
                    .First();
                if (equipment != null)
                {
                    string currentTaskState = "未知状态";

                    if (!string.IsNullOrEmpty(equipment.CurrentTaskId))
                    {
                        var task = sqlSugarClient.Queryable<HandlerAgvTask>()
                            .Where(t => t.ID == equipment.CurrentTaskId)
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
                    var trans = new RabbitMqTransaction
                    {
                        EquipmentID = equipmentId,
                        TransactionName = "UpdateClientInfo",
                        ExpireSecond = 5,
                        Parameters = new Dictionary<string, object>
                            {
                                { "InputTrayCount", equipment.InputTrayNumber },
                                { "OutputTrayCount", equipment.OutputTrayNumber },
                                { "AgvEnabled" , equipment.AgvEnabled },
                                { "CurrentTaskState", currentTaskState }
                            }
                    };
                    rabbitMqService.Produce("EAP.SecsClient." + equipmentId, trans);
                }
                else
                {
                    dbgLog.Warn($"EAP Client not found for Equipment ID: {equipmentId}");
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error($"Error updating EAP Client Info for Equipment ID: {equipmentId}", ex);
            }
        }

    }
}
