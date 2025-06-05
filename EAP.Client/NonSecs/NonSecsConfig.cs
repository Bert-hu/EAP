using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.NonSecs
{
    class NonSecsConfig
    {
        public bool IsActive { get; set; } = false;
        public string IpAddress { get; set; } = "0.0.0.0";
        public int Port { get; set; } = 5000;
        public int SocketReceiveBufferSize { get; set; } = 81920;
    }
}
