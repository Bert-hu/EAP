using EAP.Client.NonSecs.Message;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.NonSecs
{
    internal class NonSecsWorker : BackgroundService
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        private readonly NonSecsService nonSecsService;
        private readonly IServiceProvider serviceProvider;
        public NonSecsWorker(NonSecsService nonSecsService, IServiceProvider serviceProvider)
        {
            this.nonSecsService = nonSecsService;
            this.serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var service = nonSecsService.Start(stoppingToken);
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
                await service;
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async Task HandlePrimaryMessage(NonSecsMessageWrapper wrapper)
        {
            var streamfunction = $"PrimaryS{wrapper.Stream}F{wrapper.Function}";
            var interfaceType = typeof(IPrimaryMessageHandler);
            var type = Assembly.GetExecutingAssembly().GetTypes().Where(t => interfaceType.IsAssignableFrom(t) && t.Name == streamfunction).FirstOrDefault();
            if (type != null)
            {
                //IPrimaryMessageHandler obj = (IPrimaryMessageHandler)Activator.CreateInstance(type);
                //await obj.HandlePrimaryMessage(wrapper, _rabbitMqService, _secsGem, commonLibrary);
                using (var scope = serviceProvider.CreateAsyncScope())
                {
                    var handler = (IPrimaryMessageHandler)scope.ServiceProvider.GetRequiredService(type);
                    await handler.HandlePrimaryMessage(wrapper);
                }
            }
            else//未找到实现的类
            {
                var secondaryMessage = new NonSecsMessage(wrapper.Stream,wrapper.Function + 1)
                {
                };
                _ = wrapper.TryReplyAsync(secondaryMessage);
            }
        }
    }
}
