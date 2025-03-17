using EAP.Client.Forms;
using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Secs4Net;
using static Secs4Net.Item;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class ProcessStateChanged : IEventHandler
    {
        private static readonly ILog traLog = log4net.LogManager.GetLogger("traLog");
        private readonly ISecsGem secsGem;
        private RabbitMqService rabbitMqService;
        private readonly IServiceProvider serviceProvider;

        public static bool NeedChangeRecipe = false;
        public static string ChangeRecipeName = string.Empty;
        public static DateTime ChangeDateTime = DateTime.MinValue;


        public ProcessStateChanged(RabbitMqService rabbitMq, ISecsGem secsGem, IServiceProvider serviceProvider)
        {
            this.rabbitMqService = rabbitMq;
            this.secsGem = secsGem;
            this.serviceProvider = serviceProvider;

        }


        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var handler = (ITransactionHandler)scope.ServiceProvider.GetRequiredService(typeof(GetEquipmentStatus));
                handler.HandleTransaction(null);
            }
            int processStateCode = wrapper.PrimaryMessage.SecsItem[2][0][1][0].FirstValue<byte>();
            bool idlestate = processStateCode == 1;
            //bool recmoteControl = commonLibrary.CustomSettings["RemoteControl"]?.ToUpper() == "TRUE";

            bool autoCheckRecipe = MainForm.Instance.isAutoCheckRecipe;
            if (autoCheckRecipe)
            {
                if (idlestate && NeedChangeRecipe)
                {
                    if (!string.IsNullOrEmpty(ChangeRecipeName) && ChangeDateTime.AddMinutes(2) > DateTime.Now)
                    {
                        traLog.Info($"Send PP-SELECT COMMAND '{ChangeRecipeName}'");
                        var s2f41ppselect = new SecsMessage(2, 41)
                        {
                            SecsItem = L(
                                A("PP-SELECT"),
                                L(
                                    L(
                                    A("PPID"),
                                    A(ChangeRecipeName)
                                    )
                                    )
                                )
                        };
                        var s2f42ppselect = await secsGem.SendAsync(s2f41ppselect);
                        if (s2f42ppselect.SecsItem.Items[0].FirstValue<byte>() != 0)
                        {
                            //SendS10F3ToEquipment(secsGem, $"PP-SELECT Fail, Code: {s2f42ppselect.SecsItem.Items[0].FirstValue<byte>()}");
                            MainForm.Instance.ConfirmMessageBox($"切换机种指令被拒绝, Code: {s2f42ppselect.SecsItem.Items[0].FirstValue<byte>()}");
                            return;
                        }
                    }
                    else//大于2分钟的直接关闭切换，以免出错
                    {
                        traLog.Warn($"有超时的切换机种任务：{NeedChangeRecipe}");
                    }
                    NeedChangeRecipe = false;
                    ChangeDateTime = DateTime.MinValue;
                    ChangeRecipeName = string.Empty;
                }
            }
        }
    }
}
