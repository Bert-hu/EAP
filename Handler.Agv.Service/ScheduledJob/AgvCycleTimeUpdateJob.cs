using AutoMapper;
using HandlerAgv.Service.Models;
using HandlerAgv.Service.Models.Database;
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

    public class AgvCycleTimeUpdateJob : IJob
    {
        private static log4net.ILog Log = LogManager.GetLogger("Debug");

        public Task Execute(IJobExecutionContext context)
        {
            var configuration = context.JobDetail.JobDataMap["configuration"] as IConfiguration;
            var mapper = context.JobDetail.JobDataMap["mapper"] as IMapper;

            var sqlSugarClient = SqlsugarService.GetSqlSugarClient(configuration);


            try
            {
                MachineEstimatedService machineEstimatedService = new MachineEstimatedService(sqlSugarClient,mapper);
                machineEstimatedService.UpdateAllCycleTime();

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
