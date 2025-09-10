using AutoMapper;
using HandlerAgv.Service.RabbitMq;
using HandlerAgv.Service.ScheduledJob;
using HandlerAgv.Service.ScheduledJob.ContinuousLotMode;
using HandlerAgv.Service.ScheduledJob.SingleLotMode;
using log4net;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System.Configuration;

namespace HandlerAgv.Service.Services
{
    public class CommonWorker : BackgroundService
    {
        private readonly IConfiguration configuration;
        private readonly RabbitMqService rabbitMqService;
        private readonly DbConfigurationService dbConfigurationService;
        private readonly IMapper mapper;
        private bool _isDevelopment;
        private static log4net.ILog Log = LogManager.GetLogger("Debug");

        public CommonWorker(IConfiguration configuration, RabbitMqService rabbitMqService, IWebHostEnvironment env, DbConfigurationService dbConfigurationService, IMapper mapper)
        {
            this.configuration = configuration;
            this.rabbitMqService = rabbitMqService;
            this.dbConfigurationService = dbConfigurationService;
            this.mapper = mapper; 
            _isDevelopment = env.IsDevelopment();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                QuartzUtil.Init();
                var jobDataMap = new Dictionary<string, object>();
                //jobDataMap.Add("sqlSugarClient", sqlSugarClient);
                jobDataMap.Add("configuration", configuration);
                jobDataMap.Add("rabbitMqService", rabbitMqService);
                jobDataMap.Add("dbConfiguration", dbConfigurationService);
                jobDataMap.Add("mapper", mapper);

                if (_isDevelopment)
                {

                    //_ = QuartzUtil.AddJob<AgvCycleTimeUpdateJob>("AgvCycleTimeUpdateJob", DateTime.Now.AddSeconds(3), DateTimeOffset.MaxValue, 300000, jobDataMap);
                    //_ = QuartzUtil.AddJob<C_AgvTaskRequestJob>("AgvTaskGenerateJob", DateTime.Now.AddSeconds(3), DateTimeOffset.MaxValue, 10000, jobDataMap);
                    //_ = QuartzUtil.AddJob<AgvLockMachineJob>("AgvLockMachineJob", DateTime.Now.AddSeconds(3), DateTimeOffset.MaxValue, 5000, jobDataMap);
                    //_ = QuartzUtil.AddJob<C_AgvUnlockMachineJob>("C_AgvUnlockMachineJob", DateTime.Now.AddSeconds(3), DateTimeOffset.MaxValue, 5000, jobDataMap);
                    //_ = QuartzUtil.AddJob<AgvInventoryUpdateJob>("AgvInventoryUpdateJob", DateTime.Now.AddSeconds(3), DateTimeOffset.MaxValue, TimeSpan.FromSeconds(30), jobDataMap);
                                                           

                }
                else
                {
                    _ = QuartzUtil.AddJob<AgvCycleTimeUpdateJob>("AgvCycleTimeUpdateJob", DateTime.Now.AddSeconds(3), DateTimeOffset.MaxValue, 300000, jobDataMap);
                    _ = QuartzUtil.AddJob<C_AgvTaskRequestJob>("C_AgvTaskRequestJob", DateTime.Now.AddSeconds(3), DateTimeOffset.MaxValue, 10000, jobDataMap);
                    _ = QuartzUtil.AddJob<C_AgvLockMachineJob>("C_AgvLockMachineJob", DateTime.Now.AddSeconds(3), DateTimeOffset.MaxValue, 30000, jobDataMap);
                    _ = QuartzUtil.AddJob<C_AgvUnlockMachineJob>("C_AgvUnlockMachineJob", DateTime.Now.AddSeconds(3), DateTimeOffset.MaxValue, 5000, jobDataMap);
                    _ = QuartzUtil.AddJob<AgvInventoryUpdateJob>("AgvInventoryUpdateJob", DateTime.Now.AddSeconds(3), DateTimeOffset.MaxValue, TimeSpan.FromSeconds(30), jobDataMap);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}
