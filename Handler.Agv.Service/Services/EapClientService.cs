using AutoMapper;
using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.RabbitMq;
using log4net;
using SqlSugar;
using System.Reflection.PortableExecutable;

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

        public void UpdateClientInfo(string equipmentId, string message = "")
        {
            try
            {
                var equipment = sqlSugarClient.Queryable<HandlerEquipmentStatus>()
                    .Where(e => e.Id == equipmentId)
                    .First();
                if (equipment != null)
                {
                    string currentTaskState = "未知状态";
                    string taskRequestTime = "无";
                    if (!string.IsNullOrEmpty(equipment.CurrentTaskId))
                    {
                        var task = sqlSugarClient.Queryable<HandlerAgvTask>()
                            .Where(t => t.ID == equipment.CurrentTaskId)
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

                    var agvInventory = "未知";
                    var stockInventory = "未知";
                    var inventory = sqlSugarClient.Queryable<HandlerInventory>().Where(t => t.MaterialName == equipment.MaterialName && t.GroupName == equipment.GroupName).First();
                    if (inventory != null)
                    {
                        agvInventory = inventory.AgvQuantity == -1 ? "未知" : inventory.AgvQuantity.ToString();
                        stockInventory = (inventory.Stocker1Quantity + inventory.Stocker2Quantity).ToString();
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
                                { "CurrentTaskState", currentTaskState },
                                { "CurrentTaskRequestTime", taskRequestTime },
                                { "CurrentLot", equipment.CurrentLot },
                                { "GroupName", equipment.GroupName },
                                { "MaterialName", equipment.MaterialName },
                                { "AgvInventory", agvInventory },
                                { "StockInventory", stockInventory },
                                { "LoaderEmpty", equipment.LoaderEmpty}
                            }
                    };
                    if (!string.IsNullOrEmpty(message))
                    {
                        trans.Parameters.Add("Message", message);
                    }

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

        public bool MachineAgvUnlock(string equipmentId)
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
                    return result;
                }
                else
                {
                    dbgLog.Warn($"MachineAgvUnlock: {equipmentId} - No reply received or timeout.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.ToString());
                return false;
            }
        }

        public (bool locked, string message, string processState) GetMachineLockState(string equipmentId)
        {
            var lockState = false;
            var message = string.Empty;
            var processState = string.Empty;
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
                    processState = reply.Parameters.ContainsKey("ProcessState") ? reply.Parameters["ProcessState"].ToString() : "UNKNOWN";
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
            return (lockState, message, processState);
        }

    }
}
