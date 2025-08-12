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
    public class S2F41 : NonSecsMessage
    {
        public S2F41()
        {
            base.Stream = 2;
            base.Function = 41;
        }
        public string EQID { get; set; }
        public Command Command { get; set; }
    }
    public class Command
    {
        public string Name { get; set; }

        public Dictionary<string, string> Parameter { get; set; }
    }

}
