using Serilog;
using Simulator;
using SterlingConnect.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SterlingConnect.Logic
{
    public class Iso8583Service : IIso8583Service
    {
        public string ComposeMessage(Dictionary<string, string> isoMessage)
        {
            var isoMsg = new Iso8583Extended();
            try
            {
                if (!isoMessage.ContainsKey("0") || !Int32.TryParse(isoMessage["0"], out int messageType))
                {
                    return ("No MTI Specified");
                }

                isoMsg.MessageType = messageType switch
                {
                    410 => Iso8583Extended.MsgType._0410_ACQUIRER_REV_REQ_RSP,
                    420 => Iso8583Extended.MsgType._0420_ACQUIRER_REV_ADV,
                    421 => Iso8583Extended.MsgType._0421_ACQUIRER_REV_ADV_REP,
                    430 => Iso8583Extended.MsgType._0430_ACQUIRER_REV_ADV_RSP,
                    500 => Iso8583Extended.MsgType._0500_ACQUIRER_RECONCILE_REQ,
                    510 => Iso8583Extended.MsgType._0510_ACQUIRER_RECONCILE_REQ_RSP,
                    520 => Iso8583Extended.MsgType._0520_ACQUIRER_RECONCILE_ADV,
                    521 => Iso8583Extended.MsgType._0521_ACQUIRER_RECONCILE_ADV_REP,
                    530 => Iso8583Extended.MsgType._0530_ACQUIRER_RECONCILE_ADV_RSP,
                    600 => Iso8583Extended.MsgType._0600_ADMIN_REQ,
                    601 => Iso8583Extended.MsgType._0601_ADMIN_REQ_REP,
                    610 => Iso8583Extended.MsgType._0610_ADMIN_REQ_RSP,
                    800 => Iso8583Extended.MsgType._0800_NWRK_MNG_REQ,
                    801 => Iso8583Extended.MsgType._0801_NWRK_MNG_REQ_REP,
                    810 => Iso8583Extended.MsgType._0810_NWRK_MNG_REQ_RSP,
                    400 => Iso8583Extended.MsgType._0400_ACQUIRER_REV_REQ,
                    332 => Iso8583Extended.MsgType._0332_ISSUER_FILE_UPDATE_ADV_RSP,
                    330 => Iso8583Extended.MsgType._0330_ACQUIRER_FILE_UPDATE_ADV_RSP,
                    322 => Iso8583Extended.MsgType._0322_ISSUER_FILE_UPDATE_ADV,
                    100 => Iso8583Extended.MsgType._0100_AUTH_REQ,
                    110 => Iso8583Extended.MsgType._0110_AUTH_REQ_RSP,
                    120 => Iso8583Extended.MsgType._0120_AUTH_ADV,
                    130 => Iso8583Extended.MsgType._0130_AUTH_ADV_RSP,
                    200 => Iso8583Extended.MsgType._0200_TRAN_REQ,
                    201 => Iso8583Extended.MsgType._0201_TRAN_REQ_REP,
                    202 => Iso8583Extended.MsgType._0202_TRAN_CMP,
                    203 => Iso8583Extended.MsgType._0203_TRAN_CMP_REP,
                    212 => Iso8583Extended.MsgType._0212_TRAN_CMP_RSP,
                    220 => Iso8583Extended.MsgType._0220_TRAN_ADV,
                    221 => Iso8583Extended.MsgType._0221_TRAN_ADV_REP,
                    230 => Iso8583Extended.MsgType._0230_TRAN_ADV_RSP,
                    300 => Iso8583Extended.MsgType._0300_ACQUIRER_FILE_UPDATE_REQ,
                    310 => Iso8583Extended.MsgType._0310_ACQUIRER_FILE_UPDATE_RSP,
                    320 => Iso8583Extended.MsgType._0320_ACQUIRER_FILE_UPDATE_ADV,
                    210 => Iso8583Extended.MsgType._0210_TRAN_REQ_RSP,
                    _ => Iso8583Extended.MsgType._0000_INVALID_MSG,
                };

                isoMessage.Remove("0");

                foreach (var field in isoMessage)
                {

                    isoMsg[Convert.ToInt32(field.Key)] = field.Value;
                }

                return Encoding.UTF8.GetString(isoMsg.ToMsg());
            }
            catch (Exception ex)
            {
                var errorMessage =  (ex.InnerException?.Message ?? ex.Message);
                Log.Logger.Error(errorMessage);
                return errorMessage;
            }
        }


        public ConnectResponse MakeTransaction(ConnectRequest request)
        {
            var isoResponse = new ConnectResponse();

            try
            {
                var isoRequest = new List<byte>();
                if (request.AddHeader)
                {
                    isoRequest.Add((byte)(request.Request.Length / 256));
                    isoRequest.Add((byte)(request.Request.Length % 256));
                }
                isoRequest.AddRange(Encoding.UTF8.GetBytes(request.Request));

                using var client = new TcpClient(request.HostIp, request.HostPort)
                {
                    SendTimeout = 30000,
                    ReceiveTimeout = 30000
                };

                using var clientTransporter = RetreiveStream(client, request);

                clientTransporter.Write(isoRequest.ToArray());
                var response = new byte[2000];

                isoResponse.Size = clientTransporter.Read(response);
                var actualResponse = new List<byte>(response);
                isoResponse.Response = Encoding.UTF8.GetString(actualResponse.GetRange(request.StartIndex, isoResponse.Size - request.StartIndex).ToArray());

            }
            catch (Exception ex)
            {
                isoResponse.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
                Log.Logger.Error($"ERROR :: {isoResponse.ErrorMessage}");
            }

            return isoResponse;
        }

        private bool UserCertificator(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        private Stream RetreiveStream(TcpClient client, ConnectRequest request)
        {
            if (request.UseSSL)
            {
                using var securedStream = new SslStream(client.GetStream(), true, UserCertificator);
                securedStream.AuthenticateAsClient(request.HostIp);
                return securedStream;
            }

            return client.GetStream();
        }

        public string ParseMessage(string isoMessage)
        {
            var iso = new Iso8583Extended();
            try
            {
                iso.Unpack(Encoding.UTF8.GetBytes(isoMessage.Trim()), 0);
                return iso.ToString();
            }
            catch (Exception ex)
            {
                return (ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
