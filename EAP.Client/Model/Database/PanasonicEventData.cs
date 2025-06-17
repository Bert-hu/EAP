using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.Model.Database
{
    class PanasonicEventData
    {
        public string ID { get; set; } = Guid.NewGuid().ToString("N");
        public DateTime EventTime { get; set; }
        public string MachineNo { get; set; }
        public string Code { get; set; }
        public string SubCode { get; set; }
        public string Message { get; set; }
        public string SubMessage { get; set; }

    }
}
