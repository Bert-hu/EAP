using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.NonSecs.Models
{
    public class S1F4 : NonSecsMessage
    {
        public S1F4()
        {
            base.Stream = 1;
            base.Function = 4;
        }
        public S1F4(int stream, int function) : base(stream, function)
        {
        }
        public Dictionary<string, string>? List { get; set; }
    }
}
