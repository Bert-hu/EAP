using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.NonSecs.Models
{
    public class NonSecsMessage
    {
        public NonSecsMessage() { }
        public NonSecsMessage(int stream, int function)
        {
            Stream = stream;
            Function = function;
        }
        public int Stream { get; set; } = 0;
        public int Function { get; set; } = 0;
    }
}
