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
    public class S2F42 : NonSecsMessage
    {
        public S2F42()
        {
            base.Stream = 2;
            base.Function = 42;
        }
        public string Result { get; set; }
    }
   
}
