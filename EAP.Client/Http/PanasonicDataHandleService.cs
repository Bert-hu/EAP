using EAP.Client.Model.Database;
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
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EAP.Client.Http
{
    class PanasonicDataHandleService : BackgroundService
    {
        internal static ILog traLog = LogManager.GetLogger("Trace");

        private readonly IConfiguration configuration;
        private readonly ISqlSugarClient sqlSugarClient;

        public PanasonicDataHandleService(IConfiguration configuration, ISqlSugarClient sqlSugarClient)
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
                    
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(10000);
            }
        }

    }
}
