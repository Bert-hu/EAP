using EAP.Client.Forms;
using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using EAP.Client.Services;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Secs4Net;
using System.Windows.Forms;
using static Secs4Net.Item;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class ProcessStateChanged : IEventHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        private readonly ILog traLog = LogManager.GetLogger("Trace");


        private readonly ISecsGem secsGem;
        private RabbitMqService rabbitMqService;
        private readonly IServiceProvider serviceProvider;
        private readonly IConfiguration configuration;

        public ProcessStateChanged(RabbitMqService rabbitMq, ISecsGem secsGem, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            this.rabbitMqService = rabbitMq;
            this.secsGem = secsGem;
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;

        }


        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            try
            {
                var packageName = wrapper.PrimaryMessage.SecsItem[2][0][1][0].GetString();

                MainForm.Instance.UpdatePackageName(packageName);


                using (var scope = this.serviceProvider.CreateScope())
                {
                    var handler = (ITransactionHandler)scope.ServiceProvider.GetRequiredService(typeof(GetEquipmentStatus));
                    handler.HandleTransaction(null);
                }

                if (MainForm.Instance.machineLocked)
                {
                    var s2f41 = new SecsMessage(2, 41)
                    {
                        SecsItem = L(A("STOP"), L())
                    };
                    await secsGem.SendAsync(s2f41);
                }

                await Task.Run(() =>
                {
                    if (MainForm.Instance.autoCheckRecipe)
                    {
                        var result = RmsFunction.CompareRecipeBody(rabbitMqService, configuration, packageName);
                        if (result.Result)
                        {
                            traLog.Info(result.Message);
                        }
                        else
                        {
                            traLog.Error(result.Message);
                            var s2f41 = new SecsMessage(2, 41)
                            {
                                SecsItem = L(A("STOP"), L())
                            };
                            secsGem.SendAsync(s2f41);
                            var showMessage = DateTime.Now.ToString() + " " + result.Message;
                            var s10f3 = new SecsMessage(10, 3)
                            {
                                SecsItem = L(B(0), A(showMessage))
                            };
                            secsGem.SendAsync(s10f3);

                        }
                    }

                });
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.ToString());
            }
        }
    }
}
