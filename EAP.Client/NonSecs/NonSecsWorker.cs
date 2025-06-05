using EAP.Client.NonSecs.Models;
using log4net;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.NonSecs
{
    internal class NonSecsWorker : BackgroundService
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        private readonly NonSecsService nonSecsService;
        public NonSecsWorker(NonSecsService nonSecsService)
        {
            this.nonSecsService = nonSecsService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await nonSecsService.Start(stoppingToken);
                await foreach (var primaryMessageWrapper in nonSecsService.GetPrimaryMessageAsync(stoppingToken))
                {
                    try
                    {
                        await Task.Run(async () => await HandlePrimaryMessage(primaryMessageWrapper));
                    }
                    catch (Exception ex)
                    {
                        dbgLog.Error("Exception occurred when processing primary message", ex);
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
