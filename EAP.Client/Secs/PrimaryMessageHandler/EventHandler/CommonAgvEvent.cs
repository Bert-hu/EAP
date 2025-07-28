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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class CommonAgvEvent
    {
        internal static ILog traLog = LogManager.GetLogger("Trace");
        public static Dictionary<string, string> StatusDict = new Dictionary<string, string>
        {
            { "1", "IDLE" },
            { "2", "READY" },
            { "3", "INITIALIZING" },
            { "4", "RUNNING" },
            { "5", "PAUSE" },
            { "6", "STOP" },
            { "7", "LOCKED" },
            { "8", "TEACHING" },
            { "9", "WARNING" },
            { "10", "ERROR" },
            { "11", "ENDING_LOT" },
            { "12", "HOMING" },
            { "13", "MOTOR_TUNING" },
            { "14", "DRY_RUN_RUNNING" },
            { "15", "DRY_RUN_PAUSE" },
            { "16", "DRY_RUN_WARNING" },
            { "17", "DRY_RUN_ERROR" },
            { "18", "EXERCISE_RUNNING" },
            { "19", "EXERCISE_PAUSE" },
            { "20", "EXERCISE_WARNING" },
            { "21", "EXERCISE_ERROR" },
            { "22", "SEPARATE_COLOR_TRAY_RUNNING" }
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

                recipeName = wrapper.PrimaryMessage.SecsItem[2][0][1][0].GetString();
                processStateCode = wrapper.PrimaryMessage.SecsItem[2][0][1][1].FirstValue<byte>().ToString();
                StatusDict.TryGetValue(processStateCode, out processState);


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
                    {"RecipeName",recipeName}
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
