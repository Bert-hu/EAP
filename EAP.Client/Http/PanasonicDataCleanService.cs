using EAP.Client.Model.Database;
using EAP.Client.RabbitMq;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Linq;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static System.Windows.Forms.AxHost;

namespace EAP.Client.Http
{
    class PanasonicDataCleanService : BackgroundService
    {
        internal static ILog traLog = LogManager.GetLogger("Trace");

        private readonly IConfiguration configuration;
        private readonly ISqlSugarClient sqlSugarClient;

        public PanasonicDataCleanService(IConfiguration configuration, ISqlSugarClient sqlSugarClient)
        {
            this.configuration = configuration;
            this.sqlSugarClient = sqlSugarClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //清理15天前的数据
                    var cleanDateTime = DateTime.Now.AddDays(-15);
                    var count = sqlSugarClient.Deleteable<PanasonicEventData>(it => it.EventTime < cleanDateTime).ExecuteCommand();
                    traLog.Info($"清理{cleanDateTime}之前的{count}条数据");
                }
                catch (Exception ex)
                {
                    traLog.Error(ex.ToString());
                }
                await Task.Delay((int)TimeSpan.FromDays(1).TotalMilliseconds);
            }
        }

    }
}
