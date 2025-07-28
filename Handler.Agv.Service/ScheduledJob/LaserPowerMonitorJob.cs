using HandlerAgv.Service.Models;
using HandlerAgv.Service.Services;
using log4net;
using Microsoft.Extensions.Configuration;
using Quartz;
using System.Configuration;
using System.Text;

namespace HandlerAgv.Service.ScheduledJob
{
    public class LaserPowerMonitorJob : IJob
    {
        private static log4net.ILog Log = LogManager.GetLogger("Debug");
        private IConfiguration configuration;

        public Task Execute(IJobExecutionContext context)
        {
            return Task.CompletedTask;
        }

    }
}
