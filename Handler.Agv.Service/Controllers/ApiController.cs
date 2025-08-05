using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.Models.ViewModel;
using HandlerAgv.Service.RabbitMq;
using HandlerAgv.Service.Services;
using log4net;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace HandlerAgv.Service.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class ApiController : Controller
    {
        private ILog dbgLog = LogManager.GetLogger("Debug");
        private readonly ISqlSugarClient sqlSugarClient;
        private readonly RabbitMqService rabbitMqService;

        public ApiController(ISqlSugarClient sqlSugarClient, RabbitMqService rabbitMqService)
        {
            this.sqlSugarClient = sqlSugarClient;
            this.rabbitMqService = rabbitMqService;
        }

        /// <summary>
        /// 查询各个机种Handler设备开机数量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetMachineCountByMaterial()
        {
            var eq = sqlSugarClient.Queryable<HandlerEquipmentStatus>().ToList();

            var eqids = sqlSugarClient.Queryable<HandlerEquipmentStatus>()
                .Where(it => it.AgvEnabled == true)
                .GroupBy(it => new { it.MaterialName, it.GroupName })
                .Select(it => new
                {
                    MaterialName = it.MaterialName,
                    GroupName = it.GroupName,
                    Count = SqlFunc.AggregateCount(it.Id)
                }).ToList();
            return Json(eqids);
        }

        [HttpPost]
        public JsonResult GetEquipmentState(GetEquipmentStateRequest request)
        {
            var result = false;
            var message = string.Empty;
            try
            {
                var task = sqlSugarClient.Queryable<HandlerAgvTask>().InSingle(request.TaskId);
                if (task == null)
                {
                    message = "EAP找不到任务";
                }
                EapClientService clientService = new EapClientService(sqlSugarClient, rabbitMqService);

                if (task.Status == AgvTaskStatus.AgvRequested)//AGV到达后首次请求
                {
                    task.Status = AgvTaskStatus.AgvArrived;
                    task.AgvArriveTime = DateTime.Now;
                    task.AgvId = request.AgvId;
                    sqlSugarClient.Updateable(task).UpdateColumns(it => new { it.Status, it.AgvArriveTime, it.AgvId }).ExecuteCommand();
                    clientService.MachineAgvLock(task.EquipmentId);
                    dbgLog.InfoFormat($"GetEquipmentState: {request.TaskId}, AGV任务已到达，状态更新为{task.Status}, 发送锁定指令。");
                    clientService.UpdateClientInfo(task.EquipmentId);
                }
                else if (task.Status == AgvTaskStatus.AgvArrived)//AGV到达后再次请求
                {
                    (result, message) = clientService.GetMachineLockState(task.EquipmentId);
                    if (result)
                    {
                        dbgLog.InfoFormat($"GetEquipmentState: {request.TaskId}, 设备{task.EquipmentId}锁定成功，状态更新为MachineReady。");
                        task.Status = AgvTaskStatus.MachineReady;
                        task.MachineReadyTime = DateTime.Now;
                        task.AgvId = request.AgvId;
                        sqlSugarClient.Updateable(task).UpdateColumns(it => new { it.Status, it.MachineReadyTime, it.AgvId }).ExecuteCommand();
                        clientService.UpdateClientInfo(task.EquipmentId);
                    }
                    else
                    {
                        dbgLog.Info($"GetEquipmentState: {request.TaskId}, 设备{task.EquipmentId}还未锁定，稍后再试。");
                        clientService.MachineAgvLock(task.EquipmentId);
                    }
                }
                else if (task.Status == AgvTaskStatus.MachineReady)
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {
                dbgLog.Error($"GetEquipmentState: {request.TaskId}, Error: {ex.Message}");
                message = "EAP异常.";
            }
            dbgLog.Info($"GetEquipmentState: {request.TaskId}, {result}, {message}");
            return Json(new { Result = result, Message = message });
        }


        [HttpPost]
        public JsonResult TaskFeedBack(TaskFeedBackRequest request)
        {
            var result = false;
            var message = string.Empty;
            try
            {
                var task = sqlSugarClient.Queryable<HandlerAgvTask>().InSingle(request.TaskId);
                if (task == null)
                {
                    message = "EAP找不到任务";
                }
                else
                {
                    var machine = sqlSugarClient.Queryable<HandlerEquipmentStatus>()
                        .Where(it => it.Id == task.EquipmentId)
                        .First();

                    EapClientService clientService = new EapClientService(sqlSugarClient, rabbitMqService);
                    if (request.Result == "Finished")
                    {
                        task.Status = AgvTaskStatus.AgvRobotFinished;
                        task.CompletedTime = DateTime.Now;
                        sqlSugarClient.Updateable(task).UpdateColumns(it => new { it.Status, it.CompletedTime }).ExecuteCommand();
                        clientService.MachineAgvUnlock(task.EquipmentId);
                        dbgLog.Info($"TaskFeedBack: 设备：{task.EquipmentId}，任务ID：{request.TaskId}，已正常完成，状态更新为AgvRobotFinished。");

                        if (task.Type == AgvTaskType.Input )
                        {
                            machine.InputTrayNumber = request.LotLayers;
                            sqlSugarClient.Updateable(machine).UpdateColumns(it => new { it.InputTrayNumber }).ExecuteCommand();
                        }
                        else if(task.Type == AgvTaskType.Output)
                        {
                            machine.OutputTrayNumber = 0;
                            sqlSugarClient.Updateable(machine).UpdateColumns(it => new { it.OutputTrayNumber }).ExecuteCommand();
                        }
                        else if (task.Type == AgvTaskType.InputOutput)
                        {
                            machine.InputTrayNumber = request.LotLayers;
                            machine.OutputTrayNumber = 0;
                            sqlSugarClient.Updateable(machine).UpdateColumns(it => new { it.InputTrayNumber, it.OutputTrayNumber }).ExecuteCommand();
                        }

                        clientService.UpdateClientInfo(task.EquipmentId);
                    }
                    else if (request.Result == "Cancelled")
                    {
                        task.Status = AgvTaskStatus.AbnormalEnd;
                        task.CompletedTime = DateTime.Now;
                        sqlSugarClient.Updateable(task).UpdateColumns(it => new { it.Status, it.CompletedTime }).ExecuteCommand();
                        clientService.MachineAgvUnlock(task.EquipmentId);
                        dbgLog.Info($"TaskFeedBack: 设备：{task.EquipmentId}，任务ID：{request.TaskId}，已取消，状态更新为AbnormalEnd。");
                        clientService.UpdateClientInfo(task.EquipmentId);
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error($"TaskFeedBack: {request.TaskId}, Error: {ex.Message}");
                message = "EAP异常.";
            }
            dbgLog.Info($"TaskFeedBack: {request.TaskId}, {result}, {message}");
            return Json(new { Result = result, Message = message });
        }
    }
}
