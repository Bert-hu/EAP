using EAP.Client.Forms;
using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using Secs4Net;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class UnloadTrayOut : CommonAgvEvent, IEventHandler
    {

        private RabbitMqService rabbitMqService;
        private readonly ISecsGem secsGem;
        private readonly CommonLibrary commonLibrary;
        private readonly IServiceProvider serviceProvider;

        public UnloadTrayOut(RabbitMqService rabbitMqService, ISecsGem secsGem, CommonLibrary commonLibrary, IServiceProvider serviceProvider)
        {
            this.rabbitMqService = rabbitMqService;
            this.secsGem = secsGem;
            this.commonLibrary = commonLibrary;
            this.serviceProvider = serviceProvider;
        }

        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            HandleCommonAgvEvent(ceid, wrapper, rabbitMqService, commonLibrary);

            MainForm.Instance.OutputTrayCount = MainForm.Instance.OutputTrayCount + 1;
            await Task.Run(() => MainForm.Instance.UpdateMachineOutputTrayCount(MainForm.Instance.OutputTrayCount));
        }
    }
}
