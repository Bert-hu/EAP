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

        public void MachineAgvLock(string equipmentId)
        {
            try
            {
                var trans = new RabbitMqTransaction
                {
                    EquipmentID = equipmentId,
                    TransactionName = "AgvLock",
                    ExpireSecond = 5
                };
                var reply = rabbitMqService.ProduceWaitReply("EAP.SecsClient." + equipmentId, trans);
                if (reply != null)
                {
                    var result = reply.Parameters.ContainsKey("Result") && (bool)reply.Parameters["Result"];
                    var message = reply.Parameters.ContainsKey("Message") ? reply.Parameters["Message"].ToString() : "";
                    dbgLog.Info($"MachineAgvLock: {equipmentId}, Result: {result}, Message: {message}");
                }
                else
                {
                    dbgLog.Warn($"MachineAgvLock: {equipmentId} - No reply received or timeout.");
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.ToString());
            }
        }

        public void MachineAgvUnlock(string equipmentId)
        {
            try
            {
                var trans = new RabbitMqTransaction
                {
                    EquipmentID = equipmentId,
                    TransactionName = "AgvUnlock",
                    ExpireSecond = 5
                };
                var reply = rabbitMqService.ProduceWaitReply("EAP.SecsClient." + equipmentId, trans);
                if (reply != null)
                {
                    var result = reply.Parameters.ContainsKey("Result") && (bool)reply.Parameters["Result"];
                    var message = reply.Parameters.ContainsKey("Message") ? reply.Parameters["Message"].ToString() : "";
                    dbgLog.Info($"MachineAgvUnlock: {equipmentId}, Result: {result}, Message: {message}");
                }
                else
                {
                    dbgLog.Warn($"MachineAgvUnlock: {equipmentId} - No reply received or timeout.");
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.ToString());
            }
        }

        public (bool, string) GetMachineLockState(string equipmentId)
        {
            var lockState = false;
            var message = string.Empty;
            try
            {
                var trans = new RabbitMqTransaction
                {
                    EquipmentID = equipmentId,
                    TransactionName = "GetAgvLockState",
                    ExpireSecond = 5,
                };
                var reply = rabbitMqService.ProduceWaitReply("EAP.SecsClient." + equipmentId, trans);
                if (reply != null)
                {
                    lockState = reply.Parameters.ContainsKey("Result") && (bool)reply.Parameters["Result"];
                    message = reply.Parameters.ContainsKey("Message") ? reply.Parameters["Message"].ToString() : "获取设备锁定状态成功";
                }
                else
                {
                    message = "获取设备锁定状态超时或失败";
                }

            }
            catch (Exception ex)
            {
                message = $"获取设备锁定状态异常: {equipmentId} ,{ex.Message}";
                dbgLog.Error(ex.ToString());
            }
            return (lockState, message);
        }

    }
}
