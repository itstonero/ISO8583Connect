using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SterlingConnect.Helpers
{
    public class NoiseMaker
    {
        public static String MakeHash(String key, String message)
        {
            var keyBytes = StringToByteArray(key);
            var messageBytes = StringToByteArray(message);

            return ByteArrayToString(messageBytes);
        }

        public static String MakeMAC(String key, String message)
        {
            var keyBytes = StringToByteArray(key);
            var messageBytes = StringToByteArray(message);


            return ByteArrayToString(messageBytes);
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
