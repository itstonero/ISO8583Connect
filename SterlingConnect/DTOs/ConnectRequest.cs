using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SterlingConnect.DTOs
{
    public class ConnectRequest
    {
        public String HostIp { get; set; }
        public Int32 HostPort { get; set; }
        public bool AddHeader { get; set; }
        public bool UseSSL { get; set; }
        public Int32 StartIndex { get; set; }
        public String Request { get; set; }
    }
}
