using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.NonSecs.Message
{
    public class S1F4 : NonSecsMessage
    {
        public S1F4()
        {
            base.Stream = 1;
            base.Function = 4;
        }
        public Dictionary<string, string>? List { get; set; }
    }
}
