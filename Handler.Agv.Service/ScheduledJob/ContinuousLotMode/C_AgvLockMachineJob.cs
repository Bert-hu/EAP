using AutoMapper;
using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.RabbitMq;
using HandlerAgv.Service.Services;
using log4net;
using Quartz;

namespace HandlerAgv.Service.ScheduledJob.ContinuousLotMode
{
    [DisallowConcurrentExecution]

    public class C_AgvLockMachineJob : IJob
    {
        private ILog Log = LogManager.GetLogger("Debug");

        public Task Execute(IJobExecutionContext context)
        {
            var configuration = context.JobDetail.JobDataMap["configuration"] as IConfiguration;
            var dbConfiguration = context.JobDetail.JobDataMap["dbConfiguration"] as DbConfigurationService;
            var rabbitMqService = context.JobDetail.JobDataMap["rabbitMqService"] as RabbitMqService;
            var mapper = context.JobDetail.JobDataMap["mapper"] as IMapper;

            var sqlSugarClient = SqlsugarService.GetSqlSugarClient(configuration);

            try
            {
                var machines = sqlSugarClient.Queryable<HandlerEquipmentStatus>()
                    .Where(t => t.IsValiad && t.AgvEnabled && t.LoaderEmpty && !t.SingleLotMode)
                    .ToList();
                if (machines.Count > 0)
                {

                    EapClientService clientService = new EapClientService(sqlSugarClient, rabbitMqService);

                    foreach (var machine in machines)
                    {
                        try
                        {
                            (var result, var message, var processState) = clientService.GetMachineLockState(machine.Id);
                            if (!result)
                            {
                                Log.Info($"AgvLockMachineJob:  设备{machine.Id}未锁定，发送锁定指令。");
                                clientService.MachineAgvLock(machine.Id);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"AgvLockMachineJob: 锁定设备{machine.Id}时发生错误。", ex);
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
