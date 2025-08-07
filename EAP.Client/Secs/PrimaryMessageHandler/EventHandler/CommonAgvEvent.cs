using EAP.Client.Forms;
using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using log4net;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class CommonAgvEvent
    {
        internal static ILog traLog = LogManager.GetLogger("Trace");
        public static Dictionary<string, string> StatusDict = new Dictionary<string, string>
        {
            { "0", "INIT" },
            { "1", "IDLE" },
            { "2", "SETUP" },
            { "3", "READY" },
            { "4", "EXECUTING" },
            { "5", "PAUSE" },
            { "6", "ALARM_PAUSE" },
            { "7", "IDLE_WITH_ALARMS" },
            { "8", "EXIT" }
        };
        internal void HandleCommonAgvEvent(GemCeid ceid, PrimaryMessageWrapper wrapper, RabbitMqService rabbitMq, CommonLibrary commonLibrary)
        {
            try
            {
                var eventname = ceid.Name;
                var ceidint = (int)wrapper.PrimaryMessage.SecsItem[1].FirstValue<uint>();
                string recipeName = string.Empty;
                string processStateCode = string.Empty;
                string processState = "Unknown";
                string alarmList = string.Empty;
                bool lockState = false;
                bool cleanOut = false;
                bool auto1Full = false;

                recipeName = wrapper.PrimaryMessage.SecsItem[2][0][1][0].GetString();
                processStateCode = wrapper.PrimaryMessage.SecsItem[2][0][1][1].GetString();
                alarmList = wrapper.PrimaryMessage.SecsItem[2][0][1][2].GetString();
                lockState = wrapper.PrimaryMessage.SecsItem[2][0][1][3].GetString().ToUpper() == "TRUE";
                cleanOut = wrapper.PrimaryMessage.SecsItem[2][0][1][4].GetString().ToUpper() == "TRUE";
                auto1Full = wrapper.PrimaryMessage.SecsItem[2][0][1][5].GetString().ToUpper() == "TRUE";
                StatusDict.TryGetValue(processStateCode, out processState);
                if (MainForm.Instance != null)
                {
                    MainForm.Instance.UpdateState(processState);
                    MainForm.Instance.AgvLocked = lockState;
                }

                traLog.Debug($"{ceidint} {eventname}: {processState},{recipeName}");
                var equipmentid = commonLibrary.CustomSettings["EquipmentId"];
                var rabbitTrans = new RabbitMqTransaction()
                {
                    TransactionName = "MachineEventTrigger",
                    EquipmentID = equipmentid,
                    Parameters = new Dictionary<string, object>()
                {
                    {"EventName", eventname},
                    {"ProcessState",processState},
                    {"ProcessStateCode",processStateCode},
                    {"RecipeName",recipeName},
                    {"AlarmList", alarmList},
                    {"LockState", lockState},
                    {"CleanOut", cleanOut},
                    {"Auto1Full", auto1Full}

                },
                };
                rabbitMq.Produce("HandlerAgv.Service", rabbitTrans);
            }
            catch (Exception ex)
            {
                traLog.Error(ex.ToString());
            }
        }
    }
}
