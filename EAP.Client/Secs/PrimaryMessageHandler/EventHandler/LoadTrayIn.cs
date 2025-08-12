using EAP.Client.Forms;
using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using EAP.Client.Service;
using Microsoft.Extensions.Configuration;
using Secs4Net;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class LoadTrayIn : CommonAgvEvent, IEventHandler
    {

        private RabbitMqService rabbitMqService;
        private readonly ISecsGem secsGem;
        private readonly CommonLibrary commonLibrary;
        private readonly IServiceProvider serviceProvider;
        private readonly IConfiguration configuration;

        public LoadTrayIn(RabbitMqService rabbitMqService, ISecsGem secsGem, CommonLibrary commonLibrary, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            this.rabbitMqService = rabbitMqService;
            this.secsGem = secsGem;
            this.commonLibrary = commonLibrary;
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;
        }

        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            HandleCommonAgvEvent(ceid, wrapper, rabbitMqService, commonLibrary);

            //MainForm.Instance.InputTrayCount = MainForm.Instance.InputTrayCount > 0 ? MainForm.Instance.InputTrayCount - 1 : 0;
            MainForm.Instance.InputTrayCount = MainForm.Instance.InputTrayCount - 1;
            JhtHanderService jhtHanderService = new JhtHanderService(rabbitMqService, configuration);
            await Task.Run(() => jhtHanderService.UpdateMachineInputTrayCount(MainForm.Instance.InputTrayCount));
        }
    }
}
