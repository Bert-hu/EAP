using EAP.Client.Secs;
using EAP.Client.Secs.PrimaryMessageHandler.EventHandler;
using log4net;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Secs4Net.Item;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace EAP.Client.RabbitMq.TransactionHandler
{
    internal class PpSelect : ITransactionHandler
    {
        private readonly ILog traLog = LogManager.GetLogger("Trace");

        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsGem secsGem;
        internal readonly CommonLibrary commonLibrary;


        public PpSelect(RabbitMqService rabbitMq, ISecsGem secsGem, CommonLibrary commonLibrary)
        {
            this.rabbitMq = rabbitMq;
            this.secsGem = secsGem;
            this.commonLibrary = commonLibrary;
        }

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var reptrans = trans.GetReplyTransaction();
            try
            {
                var recipeName = string.Empty;
                if (trans.Parameters.TryGetValue("RecipeName", out object _rec)) recipeName = _rec?.ToString();
                var controlStateVID = commonLibrary.GetGemSvid("ControlState");
                var processStateVID = commonLibrary.GetGemSvid("ProcessState");
                if (!ProcessStateChanged.NeedChangeRecipe)
                {
                    SecsMessage s1f3 = new(1, 3, true)
                    {
                        SecsItem = L(
          U4((uint)controlStateVID.ID),
          U4((uint)processStateVID.ID)
          )
                    };
                    var s1f4 = await secsGem.SendAsync(s1f3);
                    var controlStateCode = s1f4.SecsItem[0].FirstValue<byte>();
                    var processStateCode = s1f4.SecsItem[1].FirstValue<byte>();
                    ProcessStateChanged.NeedChangeRecipe = true;

                    if (processStateCode != 2 && processStateCode != 4)//空闲状态，直接发切换指令
                    {
                        traLog.Info($"Send PP-SELECT COMMAND '{recipeName}'");
                        var s2f41 = new SecsMessage(2, 41)
                        {
                            SecsItem = L(
                A("PP-SELECT"),
                L(
                    L(
                          A("PPID"),
                          A(recipeName)
                        )
                    ))
                        };
                        await secsGem.SendAsync(s2f41);
                        ProcessStateChanged.OnPpSelectStatus = true;
                        ProcessStateChanged.ChangeRecipeName = recipeName;
                        ProcessStateChanged.ChangeDateTime = DateTime.Now;
                    }
                    else //非空闲状态，先发送Stop
                    {
                        if (!ProcessStateChanged.OnPpSelectStatus)
                        {
                            traLog.Info($"Send STOP COMMAND.");
                            var s2f41stop = new SecsMessage(2, 41)
                            {
                                SecsItem = L(
                                  A("STOP"),
                                  L(

                                      )
                                  )
                            };

                            var s2f42stop = await secsGem.SendAsync(s2f41stop);
                            if (s2f42stop.SecsItem.Items[0].FirstValue<byte>() != 0)
                            {
                                traLog.Info($"Machine reject stop command");
                                reptrans.Parameters.Add("Result", false);
                                reptrans.Parameters.Add("Message", "设备拒绝停止指令");

                            }
                            else
                            {

                            }
                            ProcessStateChanged.OnPpSelectStatus = false;
                            ProcessStateChanged.ChangeRecipeName = recipeName;
                            ProcessStateChanged.ChangeDateTime = DateTime.Now;
                        }
                        else
                        {
                            reptrans.Parameters.Add("Result", false);
                            reptrans.Parameters.Add("Message", "设备正在切换，请等待完成后再试");
                        }
                    }

                    reptrans.Parameters.Add("Result", true);
                    reptrans.Parameters.Add("Message", "发送指令成功！");
                }
                else
                {
                    reptrans.Parameters.Add("Result", false);
                    reptrans.Parameters.Add("Message", "正在切换，请等待后重试！");
                }

            }
            catch (Exception ex)
            {
                reptrans.Parameters.Add("Result", false);
                reptrans.Parameters.Add("Message", $"EAP Error {ex.Message}");
                traLog.Error(ex.ToString());
            }
            rabbitMq.Produce(trans.ReplyChannel, reptrans);
        }
    }
}
