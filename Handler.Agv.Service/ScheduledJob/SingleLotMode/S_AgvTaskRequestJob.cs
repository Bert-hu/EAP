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
using System.Diagnostics;
using System.Text;

namespace HandlerAgv.Service.ScheduledJob.SingleLotMode
{
    [DisallowConcurrentExecution]
    public class S_AgvTaskRequestJob : IJob
    {
        private static ILog Log = LogManager.GetLogger("Debug");

        public Task Execute(IJobExecutionContext context)
        {
            var configuration = context.JobDetail.JobDataMap["configuration"] as IConfiguration;
            var dbConfiguration = context.JobDetail.JobDataMap["dbConfiguration"] as DbConfigurationService;
            var mapper = context.JobDetail.JobDataMap["mapper"] as IMapper;
            var rabbitMqService = context.JobDetail.JobDataMap["rabbitMqService"] as RabbitMqService;
            var cancelStates = (dbConfiguration.GetConfigurations("CancelProcessStates") ?? "ALARM_PAUSE").Split(',');

            var sqlSugarClient = SqlsugarService.GetSqlSugarClient(configuration);

            try
            {
                var bufferTrayCount = int.Parse(dbConfiguration.GetConfigurations("BufferTrayCount") ?? "2");
                var bufferTime = int.Parse(dbConfiguration.GetConfigurations("BufferTime") ?? "180");

                var enableMachines = sqlSugarClient.Queryable<HandlerEquipmentStatus>()
                    .Where(x => x.AgvEnabled && x.IsValiad && x.SingleLotMode)
                    .ToList();
                MachineEstimatedService machineEstimatedService = new MachineEstimatedService(sqlSugarClient, mapper);
                var machines = machineEstimatedService.GetEquipmentVmData(enableMachines);

                machines = machines.Where(it => it.InputTrayNumber <= bufferTrayCount
                && it.LoadEstimatedTime < DateTime.Now.AddSeconds(bufferTime)
                && string.IsNullOrEmpty(it.CurrentTaskId)
                && it.InputTrayNumber >= 0
                ).ToList();

                AgvApiService agvApiService = new AgvApiService(sqlSugarClient, mapper, dbConfiguration, rabbitMqService);

                foreach (var machine in machines)
                {
                    //出料口有物料时，且机器不处于要取消任务的状态时
                    if (machine.OutputTrayNumber > 0 && !cancelStates.Contains(machine.ProcessState))
                    {
                        //条件1：入料口CT大于BufferTime，入料口盘数为0时，发送InputOutput任务
                        var isInputTrayFullAndEmpty = machine.InputTrayCT > bufferTime && machine.InputTrayNumber == 0;
                        //条件2：入料口CT小于BufferTime，不用判断入料口盘数
                        var isInputTrayCTLow = machine.InputTrayCT < bufferTime;

                        if (isInputTrayFullAndEmpty || isInputTrayCTLow)
                        {
                            (var result, var message) = agvApiService.SendInputOutputTask(machine).Result;
                            if (result)
                            {
                                EapClientService eapClient = new EapClientService(sqlSugarClient, rabbitMqService);
                                eapClient.UpdateClientInfo(machine.Id, $"当前盘数 {machine.InputTrayNumber}，状态{machine.ProcessState}，预计可上料时间 {machine.LoadEstimatedTime:yyyy-MM-dd HH:mm:ss}，已自动发送InputOutput任务。");
                            }
                        }
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
