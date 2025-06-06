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
    public class S6F11 : NonSecsMessage
    {
        public S6F11()
        {
            base.Stream = 6;
            base.Function = 11;
        }
        public string EventID { get; set; }
        public Dictionary<string,string> Reports { get; set; }
       
    }
}
