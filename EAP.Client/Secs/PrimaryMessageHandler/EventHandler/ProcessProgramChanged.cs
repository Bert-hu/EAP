using EAP.Client.Forms;
using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using log4net;
using Secs4Net;
using static Secs4Net.Item;


namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class ProcessProgramChanged : IEventHandler
    {
        private static readonly ILog traLog = log4net.LogManager.GetLogger("Trace");
        private readonly ISecsGem secsGem;
        private readonly CommonLibrary commonLibrary;
        private RabbitMqService rabbitMqService;

        public ProcessProgramChanged(RabbitMqService rabbitMq, ISecsGem secsGem, CommonLibrary commonLibrary)
        {
            this.rabbitMqService = rabbitMq;
            this.secsGem = secsGem;
            this.commonLibrary = commonLibrary;
        }


        public Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            //var recipename = wrapper.PrimaryMessage.SecsItem[2][0][1][0].GetString();
            //bool recmoteControl = commonLibrary.CustomSettings["RemoteControl"]?.ToUpper() == "TRUE";
            {
                var linkedRecipeName = ProcessStateChanged.ChangeRecipeName;
                if (ProcessStateChanged.NeedChangeRecipe && ProcessStateChanged.OnPpSelectStatus)
                {
                    if (ProcessStateChanged.ChangeDateTime.AddMinutes(2) > DateTime.Now)
                    {
                        //if (linkedRecipeName != recipename + ".recipe")
                        //{
                        //    traLog.Warn($"Recipe mismatch: {linkedRecipeName} != {recipename + ".recipe"}");
                        //}
                        //else
                        {
                            traLog.Info($"Auto PP-SELECT success, Send START COMMAND");
                            SendPanelStartCommand(secsGem);
                        }
                    }
                    else//大于3分钟的xml报告直接关闭切换，以免出错
                    {
                        traLog.Warn($"有超时的切换机种任务：{ProcessStateChanged.ChangeRecipeName}");

                    }
                    ProcessStateChanged.NeedChangeRecipe = false;
                    ProcessStateChanged.OnPpSelectStatus = false;
                    ProcessStateChanged.ChangeRecipeName = string.Empty;
                    ProcessStateChanged.ChangeDateTime = DateTime.MinValue;
                }
              
            }

            return Task.CompletedTask;
        }

        private void SendPanelStartCommand(ISecsGem secs)
        {
            //TODO: Send S2F41 BYPASS-CIRCUIT
            var s2f41start = new SecsMessage(2, 41)
            {
                SecsItem = L(A("START"), L())
            };
            secs.SendAsync(s2f41start);
        }
    }
}
