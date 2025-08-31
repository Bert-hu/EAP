using AutoMapper;
using HandlerAgv.Service.Models;
using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.RabbitMq;
using HandlerAgv.Service.Services;
using ICOSEAP.Api.Services;
using log4net;
using Microsoft.Extensions.Configuration;
using Quartz;
using SqlSugar;
using System.Configuration;
using System.Text;

namespace HandlerAgv.Service.ScheduledJob
{
    [DisallowConcurrentExecution]
    public class AgvTaskRequestJob : IJob
    {
        private static log4net.ILog Log = LogManager.GetLogger("Debug");

        public Task Execute(IJobExecutionContext context)
        {
            var configuration = context.JobDetail.JobDataMap["configuration"] as IConfiguration;
            var dbConfiguration = context.JobDetail.JobDataMap["dbConfiguration"] as DbConfigurationService;
            var mapper = context.JobDetail.JobDataMap["mapper"] as IMapper;
            var rabbitMqService = context.JobDetail.JobDataMap["rabbitMqService"] as RabbitMqService;

            var sqlSugarClient = SqlsugarService.GetSqlSugarClient(configuration);

            try
            {
                var bufferTrayCount = int.Parse(dbConfiguration.GetConfigurations("BufferTrayCount") ?? "2");
                var bufferTime = int.Parse(dbConfiguration.GetConfigurations("BufferTime") ?? "180");

                var enableMachines = sqlSugarClient.Queryable<HandlerEquipmentStatus>()
                    .Where(x => x.AgvEnabled && x.IsValiad)
                    .ToList();
                MachineEstimatedService machineEstimatedService = new MachineEstimatedService(sqlSugarClient, mapper);
                var machines = machineEstimatedService.GetEquipmentVmData(enableMachines);

                machines = machines.Where(it => it.InputTrayNumber <= bufferTrayCount
                && it.LoadEstimatedTime < DateTime.Now.AddSeconds(bufferTime)
                && string.IsNullOrEmpty(it.CurrentTaskId)
                && it.InputTrayNumber > 0
                ).ToList();

                AgvApiService agvApiService = new AgvApiService(sqlSugarClient, mapper, dbConfiguration,rabbitMqService);

                foreach (var machine in machines)
                {
                    if (machine.OutputTrayNumber > 0)
                    {
                        agvApiService.SendInputOutputTask(machine);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            sqlSugarClient.Dispose();
            return Task.CompletedTask;
        }
    }
}
