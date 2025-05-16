using CommunityToolkit.HighPerformance;
using EAP.Client.Forms;
using EAP.Client.Models;
using EAP.Client.Secs.Models;
using EAP.Client.Sfis;
using EAP.Client.Utils;
using log4net;
using Microsoft.Extensions.Configuration;
using Secs4Net;
using System.Configuration;
using static Secs4Net.Item;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class InButtonPressed : IEventHandler
    {
        internal static ILog traLog = LogManager.GetLogger("Trace");
        private readonly ISecsGem secsGem;
        private readonly IConfiguration configuration;
        public InButtonPressed(ISecsGem secsGem,IConfiguration configuration)
        {
            this.secsGem = secsGem;
            this.configuration = configuration;
        }

        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            if (MainForm.Instance.AllowInput == MainForm.InputStatus.Allow)
            {
                //TODO:过站

                var empno = MainForm.Instance.uiTextBox_empNo.Text;
                var line = MainForm.Instance.uiTextBox_line.Text;
                ConfigManager<SputtereConfig> manager = new ConfigManager<SputtereConfig>();
                var config = manager.LoadConfig().CathodeSettings;
                var snInfos = MainForm.Instance.snInfos;
                var modelName = MainForm.Instance.uiTextBox_modelName.Text;
                var trayId = MainForm.Instance.uiTextBox_trayId.Text;
                var baymaxIp = configuration.GetSection("Custom")["SfisIp"];
                var baymaxPort = int.Parse(configuration.GetSection("Custom")["SfisPort"] ?? "21347");


                var equipmentId = configuration.GetSection("Custom")["EquipmentId"];
                string cathodeStr = string.Join(" ", config.Select(it => $"CATHODE_{it.Seq}={it.CathodeId}"));
                string step2Req = $@"{equipmentId},{snInfos.First().CarrierId},2,{empno},{line},,OK,,,ACTUAL_GROUP=SPUTTER {cathodeStr} ,,,{string.Join(";", snInfos.Select(it => it.CarrierId))},{trayId},,{modelName}";
                BaymaxService baymaxService = new BaymaxService();
                var trans = await baymaxService.GetBaymaxTrans(baymaxIp, baymaxPort, step2Req);
                if (trans.Result && trans.BaymaxResponse.ToUpper().StartsWith("OK"))
                {
                    var s2f41 = new SecsMessage(2, 41)
                    {
                        SecsItem = L(A("CHB1_ALLOW"), L())
                    };
                    var s2f42 = await secsGem.SendAsync(s2f41);
                }
                else
                {
                    traLog.Error($"过站异常：{trans.BaymaxResponse}");
                    var s2f41 = new SecsMessage(2, 41)
                    {
                        SecsItem = L(A("CHB1_REJECT"), L())
                    };
                    var s2f42 = await secsGem.SendAsync(s2f41);
                }
            }
            else
            {

                var s2f41 = new SecsMessage(2, 41)
                {
                    SecsItem = L(A("CHB1_REJECT"), L())
                };
                var s2f42 = await secsGem.SendAsync(s2f41);
            }


        }

    }
}
