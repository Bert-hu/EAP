using AutoMapper;
using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.Models.ViewModel;
using HandlerAgv.Service.RabbitMq;
using HandlerAgv.Service.Services;
using ICOSEAP.Api.Services;
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
        private readonly DbConfigurationService dbConfiguration;
        private readonly IMapper mapper;

        public ApiController(ISqlSugarClient sqlSugarClient, RabbitMqService rabbitMqService, DbConfigurationService dbConfiguration, IMapper mapper)
        {
            this.sqlSugarClient = sqlSugarClient;
            this.rabbitMqService = rabbitMqService;
            this.dbConfiguration = dbConfiguration;
            this.mapper = mapper;
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
            string command = "WAIT";
            try
            {
                var task = sqlSugarClient.Queryable<HandlerAgvTask>().InSingle(request.TaskId);
                if (task == null)
                {
                    message = "EAP找不到任务";
                }
                else
                {
                    var equipment = sqlSugarClient.Queryable<HandlerEquipmentStatus>().InSingle(task.EquipmentId);
                    MachineEstimatedService machineEstimatedService = new MachineEstimatedService(sqlSugarClient, mapper);
                    var eqpVM = machineEstimatedService.GetEquipmentVmData(new List<HandlerEquipmentStatus> { equipment }).FirstOrDefault();


                    var cancelStates = (dbConfiguration.GetConfigurations("CancelProcessStates") ?? "ALARM_PAUSE").Split(',');

                    EapClientService clientService = new EapClientService(sqlSugarClient, rabbitMqService);
                    (var lockstate, message, var processState) = clientService.GetMachineLockState(task.EquipmentId);

                    if (
                        ////设备处于报警状态
                        //cancelStates.Contains(processState)
                        ////设备预计可上料时间大于5分钟
                        //|| 
                        (eqpVM.InputTrayNumber > 0 && eqpVM.LoadEstimatedTime > DateTime.Now.AddMinutes(5))
                        )
                    {
                        dbgLog.InfoFormat($"GetEquipmentState: {request.TaskId}, 设备{task.EquipmentId}状态为{processState}，预计可上料时间为{((DateTime)eqpVM.LoadEstimatedTime - DateTime.Now).TotalMinutes.ToString("F2")}分钟后，需要AGV取消任务。");
                        command = "CANCEL";
                        if (task.AgvId != request.AgvId)
                        {
                            task.AgvArriveTime = DateTime.Now;
                            task.AgvId = request.AgvId;
                            sqlSugarClient.Updateable(task).UpdateColumns(it => new { it.AgvArriveTime, it.AgvId }).ExecuteCommand();
                        }
                    }
                    else//如果设备状态不需要取消，则继续执行AGV到达逻辑
                    {
                        if (task.Status == AgvTaskStatus.AgvRequested)//AGV到达后首次请求
                        {
                            task.Status = AgvTaskStatus.AgvArrived;
                            task.AgvArriveTime = DateTime.Now;
                            task.AgvId = request.AgvId;
                            sqlSugarClient.Updateable(task).UpdateColumns(it => new { it.Status, it.AgvArriveTime, it.AgvId }).ExecuteCommand();
                            if (equipment.LoaderEmpty)
                            {
                                clientService.MachineAgvLock(task.EquipmentId);
                                command = "WAIT";
                                dbgLog.InfoFormat($"GetEquipmentState: {request.TaskId}, AGV已到达，检测到LoaderEmpty为{equipment.LoaderEmpty}，已发送锁定指令。");
                            }
                            else
                            {
                                command = "WAIT";
                                dbgLog.InfoFormat($"GetEquipmentState: {request.TaskId}, AGV已到达，检测到LoaderEmpty为{equipment.LoaderEmpty}，暂不发送锁定指令。");
                            }
                            clientService.UpdateClientInfo(task.EquipmentId, $"AGV{request.AgvId}已到达");
                        }
                        else if (task.Status == AgvTaskStatus.AgvArrived)//AGV到达后再次请求
                        {
                            if (equipment.LoaderEmpty || task.Type != AgvTaskType.InputOutput)
                            {
                                if (lockstate)
                                {
                                    dbgLog.InfoFormat($"GetEquipmentState:{task.Type.ToString()}任务 {request.TaskId}, 设备{task.EquipmentId}锁定成功，状态更新为MachineReady。");
                                    task.Status = AgvTaskStatus.MachineReady;
                                    task.MachineReadyTime = DateTime.Now;
                                    task.AgvId = request.AgvId;
                                    sqlSugarClient.Updateable(task).UpdateColumns(it => new { it.Status, it.MachineReadyTime, it.AgvId }).ExecuteCommand();
                                    clientService.UpdateClientInfo(task.EquipmentId, $"检查到设备已锁定，运行AGV{request.AgvId}继续");
                                    result = true;
                                    command = "READY";
                                }
                                else
                                {
                                    dbgLog.Info($"GetEquipmentState: {request.TaskId}, 设备{task.EquipmentId}还未锁定，稍后再试。");
                                    clientService.MachineAgvLock(task.EquipmentId);
                                    command = "WAIT";
                                }
                            }
                            else
                            {
                                dbgLog.InfoFormat($"GetEquipmentState: {request.TaskId}, 检测到LoaderEmpty为{equipment.LoaderEmpty}，暂不发送锁定指令。");
                                command = "WAIT";
                            }                               
                        }
                        else if (task.Status == AgvTaskStatus.MachineReady)
                        {
                            result = true;
                            command = "READY";
                            clientService.UpdateClientInfo(task.EquipmentId, $"检查到设备已锁定，运行AGV{request.AgvId}继续");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                dbgLog.Error($"GetEquipmentState: {request.TaskId}, Error: {ex.Message}");
                message = "EAP异常.";
                command = "WAIT";
            }
            dbgLog.Info($"GetEquipmentState: {request.TaskId}, {result},{command}, {message}");
            return Json(new GetEquipmentStateResponse { Result = result, Message = message, Command = command });
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
                        task.AgvRobotFinishedTime = DateTime.Now;
                        sqlSugarClient.Updateable(task).UpdateColumns(it => new { it.Status, it.AgvRobotFinishedTime }).ExecuteCommand();

                        dbgLog.Info($"TaskFeedBack: 设备：{task.EquipmentId}，任务ID：{request.TaskId}，已正常完成，状态更新为AgvRobotFinished。");

                        if (task.Type == AgvTaskType.Input)
                        {
                            if (request.LotLayers == null) return Json(new { Result = false, Message = "缺少盘数信息" });

                            machine.InputTrayNumber = (int)request.LotLayers;
                            machine.CurrentLot = request.InputLot;
                            machine.LoaderEmpty = false;
                            sqlSugarClient.Updateable(machine).UpdateColumns(it => new { it.InputTrayNumber, it.CurrentLot, it.LoaderEmpty }).ExecuteCommand();
                            dbgLog.Info($"{machine.Id} 更新上料口盘数 {machine.InputTrayNumber}, 当前Lot {request.InputLot}");


                        }
                        else if (task.Type == AgvTaskType.Output)
                        {
                            machine.OutputTrayNumber = 0;
                            sqlSugarClient.Updateable(machine).UpdateColumns(it => new { it.OutputTrayNumber }).ExecuteCommand();
                            dbgLog.Info($"{machine.Id} 更新出料口盘数 0");
                        }
                        else if (task.Type == AgvTaskType.InputOutput)
                        {
                            if (request.LotLayers == null) return Json(new { Result = false, Message = "缺少盘数信息" });

                            machine.InputTrayNumber = (int)request.LotLayers;
                            machine.CurrentLot = request.InputLot;
                            machine.OutputTrayNumber = 0;
                            machine.LoaderEmpty = false;
                            sqlSugarClient.Updateable(machine).UpdateColumns(it => new { it.InputTrayNumber, it.OutputTrayNumber, it.CurrentLot, it.LoaderEmpty }).ExecuteCommand();
                            dbgLog.Info($"{machine.Id} 更新上料口盘数 {machine.InputTrayNumber}, 当前Lot {request.InputLot}， 出料口盘数 0");
                        }

                        //解锁
                        if (clientService.MachineAgvUnlock(task.EquipmentId))
                        {
                            task.Status = AgvTaskStatus.Completed;
                            task.CompletedTime = DateTime.Now;
                            sqlSugarClient.Updateable(task).UpdateColumns(it => new { it.Status, it.CompletedTime }).ExecuteCommand();
                            if (machine.CurrentTaskId == request.TaskId)
                            {
                                machine.CurrentTaskId = null;
                                sqlSugarClient.Updateable(machine).UpdateColumns(it => new { it.CurrentTaskId }).ExecuteCommand();
                                if (task.Type == AgvTaskType.Input || task.Type == AgvTaskType.InputOutput)
                                {
                                    clientService.SendStartCommand(task.EquipmentId);
                                }
                            }
                        }
                        clientService.UpdateClientInfo(task.EquipmentId, $"{task.Type.ToString()}任务{request.TaskId}已完成");
                    }
                    else if (request.Result == "Cancelled")
                    {
                        task.Status = AgvTaskStatus.AbnormalEnd;
                        task.AgvRobotFinishedTime = DateTime.Now;
                        sqlSugarClient.Updateable(task).UpdateColumns(it => new { it.Status, it.AgvRobotFinishedTime }).ExecuteCommand();
                        //20250908 任务取消，不解锁
                        //clientService.MachineAgvUnlock(task.EquipmentId);
                        dbgLog.Info($"TaskFeedBack: 设备：{task.EquipmentId}，任务ID：{request.TaskId}，已取消，状态更新为AbnormalEnd。");
                        clientService.UpdateClientInfo(task.EquipmentId, $"{task.Type.ToString()}任务{request.TaskId}已取消");
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
