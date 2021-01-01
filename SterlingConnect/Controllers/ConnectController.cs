using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Simulator;
using SterlingConnect.DTOs;
using SterlingConnect.Helpers;
using SterlingConnect.Logic;

namespace SterlingConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectController : ControllerBase
    {
        ILogger<ConnectController> Logger { get; set; }
        IIso8583Service Logic { get; set; }
        public ConnectController(ILogger<ConnectController> logger, IIso8583Service service)
        {
            Logger = logger;
            Logic = service;
        }
        
        [HttpPost]
        [Route("/GetTransfer")]
        public ActionResult<ConnectResponse> Transfer(ConnectRequest request)
        {
            Logger.LogInformation($"REQUEST :: {JsonConvert.SerializeObject(request)}");

            var response = Logic.MakeTransaction(request);
                        
            Logger.LogInformation($"RESPONSE :: {JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }


        [HttpPost]
        [Route("/MakeRequest")]
        public ActionResult<String> MakeMessage(Dictionary<String, String> isoMessage)
        {
            Logger.LogInformation($"REQUEST :: {JsonConvert.SerializeObject(isoMessage)}");

            var response = Logic.ComposeMessage(isoMessage);

            Logger.LogInformation($"RESPONSE :: {JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

        [HttpPost]
        [Route("/ParseRequest")]
        public ActionResult<String> ParseRequest(Dictionary<String, String> request)
        {
            String isoMessage = request["data"];
            Logger.LogInformation($"REQUEST :: {isoMessage}");

            var response = Logic.ParseMessage(isoMessage);

            Logger.LogInformation($"RESPONSE :: {response}");
            return Ok(response);
        }

        [HttpPost]
        [Route("/CreateHash")]
        public ActionResult<String> HashRequest(NoiseRequest request)
        {
            Logger.LogInformation($"REQUEST KEY :: {JsonConvert.SerializeObject(request)}");

            var response = NoiseMaker.MakeHash(request.Key, request.Message);

            Logger.LogInformation($"RESPONSE :: {response}");
            return Ok(String.Empty);
        }

        [HttpPost]
        [Route("/CreateMac")]
        public ActionResult<String> MacRequest(NoiseRequest request)
        {
            Logger.LogInformation($"REQUEST KEY :: {JsonConvert.SerializeObject(request)}");

            var response = NoiseMaker.MakeMAC(request.Key, request.Message);

            Logger.LogInformation($"RESPONSE :: {response}");
            return Ok(String.Empty);
        }

    }
}
