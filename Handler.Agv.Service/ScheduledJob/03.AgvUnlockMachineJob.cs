using AutoMapper;
using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.RabbitMq;
using HandlerAgv.Service.Services;
using log4net;
using Quartz;

namespace HandlerAgv.Service.ScheduledJob
{
    /// <summary>
    /// 解锁设备任务
    /// 用于AGV Robot任务完成后，或者任务异常结束后，解锁设备。
    /// </summary>
    [DisallowConcurrentExecution]
    public class AgvUnlockMachineJob : IJob
    {
        private log4net.ILog Log = LogManager.GetLogger("Debug");

        public Task Execute(IJobExecutionContext context)
        {
            var configuration = context.JobDetail.JobDataMap["configuration"] as IConfiguration;
            var dbConfiguration = context.JobDetail.JobDataMap["dbConfiguration"] as DbConfigurationService;
            var rabbitMqService = context.JobDetail.JobDataMap["rabbitMqService"] as RabbitMqService;
            var mapper = context.JobDetail.JobDataMap["mapper"] as IMapper;

            var sqlSugarClient = SqlsugarService.GetSqlSugarClient(configuration);

            try
            {
                var taskIds = sqlSugarClient.Queryable<HandlerEquipmentStatus>()
                    .Where(t => t.IsValiad && t.AgvEnabled && !string.IsNullOrEmpty(t.CurrentTaskId))
                    .ToList().Select(it => it.CurrentTaskId).ToList();
                if (taskIds.Count > 0)
                {
                    var tasks = sqlSugarClient.Queryable<HandlerAgvTask>()
                 .Where(t => taskIds.Contains(t.ID))
                 .ToList();

                    tasks = tasks.Where(t => t.Status == AgvTaskStatus.AgvRobotFinished || t.Status == AgvTaskStatus.AgvRobotAbnormal || t.Status == AgvTaskStatus.Completed || t.Status == AgvTaskStatus.AbnormalEnd).ToList();
                    EapClientService clientService = new EapClientService(sqlSugarClient, rabbitMqService);

                    foreach (var task in tasks)
                    {
                        try
                        {
                            (var result, var message, var processState) = clientService.GetMachineLockState(task.EquipmentId);
                            if (result)
                            {
                                var machine = sqlSugarClient.Queryable<HandlerEquipmentStatus>()
                                    .Where(it => it.Id == task.EquipmentId)
                                    .First();
                                if (task.Status == AgvTaskStatus.AgvRobotAbnormal || task.Status == AgvTaskStatus.AbnormalEnd)
                                {
                                    Log.Info($"AgvUnlockMachineJob: {task.ID}, 设备{task.EquipmentId}已解锁，状态更新为AbnormalEnd。");
                                    task.Status = AgvTaskStatus.AbnormalEnd;
                                    task.CompletedTime = DateTime.Now;
                                    sqlSugarClient.Updateable(task).UpdateColumns(it => new { it.Status, it.CompletedTime }).ExecuteCommand();

                                    machine.CurrentTaskId = null;
                                    sqlSugarClient.Updateable(machine).UpdateColumns(it => new { it.CurrentTaskId }).ExecuteCommand();
                                    clientService.UpdateClientInfo(task.EquipmentId);
                                }
                                else if (task.Status == AgvTaskStatus.AgvRobotFinished || task.Status == AgvTaskStatus.Completed)
                                {
                                    Log.Info($"AgvUnlockMachineJob: {task.ID}, 设备{task.EquipmentId}已解锁成功，状态更新为Completed。");
                                    task.Status = AgvTaskStatus.Completed;
                                    task.CompletedTime = DateTime.Now;
                                    sqlSugarClient.Updateable(task).UpdateColumns(it => new { it.Status, it.CompletedTime }).ExecuteCommand();

                                    machine.CurrentTaskId = null;
                                    sqlSugarClient.Updateable(machine).UpdateColumns(it => new { it.CurrentTaskId }).ExecuteCommand();
                                    clientService.UpdateClientInfo(task.EquipmentId);
                                }
                            }
                            else
                            {
                                Log.Info($"AgvUnlockMachineJob: {task.ID}, 设备{task.EquipmentId}未解锁，发送解锁指令。");
                                clientService.MachineAgvUnlock(task.EquipmentId);
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error($"AgvUnlockMachineJob: 处理任务 {task.ID} 时发生错误：{e.Message}", e);
                            continue; // 继续处理下一个任务
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return Task.CompletedTask;
        }

    }
}
