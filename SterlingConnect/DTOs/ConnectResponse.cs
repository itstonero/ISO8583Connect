using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SterlingConnect.DTOs
{
    public class ConnectResponse
    {
        public ConnectResponse()
        {
            Size = 0;
        }

        public String Response { get; set; }
        public Int32 Size { get; set; }
        public String ErrorMessage { get; set; }
    }
}
