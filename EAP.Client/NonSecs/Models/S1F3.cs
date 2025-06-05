using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.NonSecs.Models
{
    public class S1F3 : NonSecsMessage
    {
        public S1F3()
        {
            base.Stream = 1;
            base.Function = 3;
        }
        public S1F3(int stream, int function) : base(stream, function)
        {
        }
        public List<string>? List { get; set; }
    }
}
