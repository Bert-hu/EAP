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
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        private readonly ISecsGem secsGem;
        private RabbitMqService rabbitMqService;
        private readonly IServiceProvider serviceProvider;

        public ProcessStateChanged(RabbitMqService rabbitMq, ISecsGem secsGem, IServiceProvider serviceProvider)
        {
            this.rabbitMqService = rabbitMq;
            this.secsGem = secsGem;
            this.serviceProvider = serviceProvider;

        }


        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            try
            {
                var packageName = wrapper.PrimaryMessage.SecsItem[2][0][1][0].GetString();

                MainForm.Instance.UpdateMachineRecipe(packageName);


                using (var scope = this.serviceProvider.CreateScope())
                {
                    var handler = (ITransactionHandler)scope.ServiceProvider.GetRequiredService(typeof(GetEquipmentStatus));
                    handler.HandleTransaction(null);
                }

                if (MainForm.Instance.machineLocked)
                {
                    var s2f41 = new SecsMessage(2, 41)
                    {
                        SecsItem = L(
    A("STOP"),
    L()
)
                    };
                    await secsGem.SendAsync(s2f41);
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.ToString());
            }
        }
    }
}
