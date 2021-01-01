using SterlingConnect.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SterlingConnect.Logic
{
    public interface IIso8583Service
    {
        ConnectResponse MakeTransaction(ConnectRequest request);
        String ComposeMessage(Dictionary<String, String> isoMessage);
        String ParseMessage(String isoMessage);
    }
}
