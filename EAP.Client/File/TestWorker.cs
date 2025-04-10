using EAP.Client.RabbitMq;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Secs4Net;
using System.Reflection;


namespace EAP.Client.File
{
    internal partial class TestWorker : BackgroundService
    {

        private readonly IConfiguration configuration;
        private readonly ISecsGem secsGem;
        private readonly ISecsConnection hsmsConnection;


        public TestWorker(IConfiguration configuration, ISecsConnection hsmsConnection, ISecsGem secsGem,RabbitMq.RabbitMqService rabbitMqService, IServiceProvider serviceProvider)
        {
            this.configuration = configuration;
            this.secsGem = secsGem;
            this.hsmsConnection = hsmsConnection;


        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

        }



    }
}
