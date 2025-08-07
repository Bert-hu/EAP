using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using Secs4Net;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class CleanOut : CommonAgvEvent, IEventHandler
    {

        private RabbitMqService rabbitMqService;
        private readonly ISecsGem secsGem;
        private readonly CommonLibrary commonLibrary;
        private readonly IServiceProvider serviceProvider;

        public CleanOut(RabbitMqService rabbitMqService, ISecsGem secsGem, CommonLibrary commonLibrary, IServiceProvider serviceProvider)
        {
            this.rabbitMqService = rabbitMqService;
            this.secsGem = secsGem;
            this.commonLibrary = commonLibrary;
            this.serviceProvider = serviceProvider;
        }

        public Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            HandleCommonAgvEvent(ceid, wrapper, rabbitMqService, commonLibrary);

            return Task.CompletedTask;
        }
    }
}
