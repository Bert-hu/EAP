using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EAP.Client.NonSecs.Message
{
    public class S1F3 : NonSecsMessage
    {
        public S1F3()
        {
            base.Stream = 1;
            base.Function = 3;
        }

        public List<string>? List { get; set; }
    }
}
