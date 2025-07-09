using EAP.Client.Forms;
using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using EAP.Client.Sfis;
using log4net;
using Microsoft.Extensions.Configuration;
using Secs4Net;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{


    internal class Press1EndCurning : PressXEndCurning, IEventHandler
    {
        public Press1EndCurning(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            await HandlePressXEndCurning(1, ceid, wrapper);
        }

    }
    internal class PressXEndCurning
    {
        internal static ILog traLog = LogManager.GetLogger("Trace");

        private readonly IConfiguration configuration;

        public PressXEndCurning(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Task HandlePressXEndCurning(int pressid, GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            try
            {
                var leftpanelid = wrapper.PrimaryMessage.SecsItem[2][0][1][0].GetString();
                if (!string.IsNullOrEmpty(leftpanelid))
                {
                    EndCurning(leftpanelid, pressid, "LEFT");
                }
            }
            catch (Exception ex)
            {
                traLog.Error($"Press{pressid}EndCuring left Error: {ex}");
            }
            try
            {
                var rightpanelid = wrapper.PrimaryMessage.SecsItem[2][0][1][1].GetString();
                if (!string.IsNullOrEmpty(rightpanelid))
                {
                    EndCurning(rightpanelid, pressid, "RIGHT");
                }
            }
            catch (Exception ex)
            {
                traLog.Error($"Press{pressid}EndCuring right Error: {ex}");
            }

            return Task.CompletedTask;
        }

        public void EndCurning(string panelid, int pressid, string locationid)
        {
            var equipmentId = configuration.GetSection("Custom")["EquipmentId"];
            var sfisIp = configuration.GetSection("Custom")["SfisIp"];
            var sfisPort = int.Parse(configuration.GetSection("Custom")["SfisPort"] ?? "21347");

            var panelout = $"{equipmentId},{panelid},3,{MainForm.Instance.uiTextBox_empNo.Text},{MainForm.Instance.uiTextBox_line.Text},,OK,PRESS={pressid} LOCATION={locationid},,ACTUAL_GROUP={MainForm.Instance.uiTextBox_groupName.Text},,,,,,{MainForm.Instance.uiTextBox_modelName.Text},";

            BaymaxService baymax = new BaymaxService();
            baymax.GetBaymaxTrans(sfisIp, sfisPort, panelout);
        }
    }

}
