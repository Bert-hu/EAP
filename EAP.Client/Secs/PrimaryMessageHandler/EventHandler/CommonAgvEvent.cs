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
            { "1", "INIT" },
            { "2", "IDLE" },
            { "3", "SETUP" },
            { "4", "READY" },
            { "5", "EXECUTING" },
            { "6", "PAUSE" },
            { "7", "ALARM_PAUSE" },
            { "8", "IDLE_WITH_ALARMS" },
            { "9", "EXIT" }
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
                processStateCode = wrapper.PrimaryMessage.SecsItem[2][0][1][1].GetString();
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
