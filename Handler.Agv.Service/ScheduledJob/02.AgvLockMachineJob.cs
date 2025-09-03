using AutoMapper;
using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.RabbitMq;
using HandlerAgv.Service.Services;
using log4net;
using Quartz;

namespace HandlerAgv.Service.ScheduledJob
{
    [DisallowConcurrentExecution]

    public class AgvLockMachineJob : IJob
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

                    tasks = tasks.Where(t => t.Status == AgvTaskStatus.AgvArrived).ToList();
                    EapClientService clientService = new EapClientService(sqlSugarClient, rabbitMqService);

                    foreach (var task in tasks)
                    {
                        try
                        {
                            (var result, var message,var processState) = clientService.GetMachineLockState(task.EquipmentId);
                            if (result)
                            {
                                Log.Info($"AgvLockMachineJob: {task.ID}, 设备{task.EquipmentId}已锁定成功，状态更新为MachineReady。");
                                task.Status = AgvTaskStatus.MachineReady;
                                task.MachineReadyTime = DateTime.Now;
                                sqlSugarClient.Updateable(task).UpdateColumns(it => new { it.Status, it.MachineReadyTime }).ExecuteCommand();
                                clientService.UpdateClientInfo(task.EquipmentId);
                            }
                            else
                            {
                                Log.Info($"AgvLockMachineJob: {task.ID}, 设备{task.EquipmentId}未锁定，发送锁定指令。");
                                clientService.MachineAgvLock(task.EquipmentId);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"AgvLockMachineJob: {task.ID}, 锁定设备{task.EquipmentId}时发生错误。", ex);
                            continue;
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
