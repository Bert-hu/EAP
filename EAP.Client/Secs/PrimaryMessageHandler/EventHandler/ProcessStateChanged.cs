using EAP.Client.Forms;
using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Secs4Net;
using static Secs4Net.Item;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class ProcessStateChanged : IEventHandler
    {
        private readonly ISecsGem secsGem;
        private RabbitMqService rabbitMqService;
        private readonly IServiceProvider serviceProvider;

        public ProcessStateChanged(RabbitMqService rabbitMq, ISecsGem secsGem, IServiceProvider serviceProvider)
        {
            this.rabbitMqService = rabbitMq;
            this.secsGem = secsGem;
            this.serviceProvider = serviceProvider;

        }


        public Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            //using (var scope = this.serviceProvider.CreateScope())
            //{
            //    var handler = (ITransactionHandler)scope.ServiceProvider.GetRequiredService(typeof(GetEquipmentStatus));
            //    handler.HandleTransaction(null);
            //}
            return Task.CompletedTask;
        }
    }
}
