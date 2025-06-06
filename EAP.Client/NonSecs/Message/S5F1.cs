using EAP.Client.RabbitMq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EAP.Client.NonSecs.Message
{
    public class S5F1 : NonSecsMessage
    {
        public S5F1()
        {
            base.Stream = 5;
            base.Function = 1;
        }
        public string AlarmSet { get; set; }
        public string AlarmID { get; set; }
        public string AlarmText { get; set; }
       
    }
}
