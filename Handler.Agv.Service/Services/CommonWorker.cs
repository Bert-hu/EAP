using HandlerAgv.Service.ScheduledJob;
using log4net;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System.Configuration;

namespace HandlerAgv.Service.Services
{
    public class CommonWorker : BackgroundService
    {
        private readonly IConfiguration configuration;
        private bool _isDevelopment;
        private static log4net.ILog Log = LogManager.GetLogger("Debug");

        public CommonWorker(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.configuration = configuration;
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
                if (_isDevelopment)
                {
                    //_ = QuartzUtil.AddJob<LaserPowerMonitorJob>("LaserPowerMonitorJob", DateTime.Now.AddSeconds(3), DateTimeOffset.MaxValue, 600000, jobDataMap);

                }
                else
                {
                    //_ = QuartzUtil.AddJob<LaserPowerMonitorJob>("LaserPowerMonitorJob", DateTime.Now.AddSeconds(3), DateTimeOffset.MaxValue, 300000, jobDataMap);

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}
