using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Storiveo.IsisPie
{
    public class General
    {
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string ByteArrayToBinHex(byte[] bytes)
        {
            return bytes.Select(b => b.ToString("X2")).Aggregate((s1, s2) => s1 + s2);
        }

        public static byte[] HexStringToByteArray(string data)
        {
            data = data.Trim();
            var listOfByte = new List<byte>();
            var seperator = new[] { ' ', ',', ';', ':' };
            var strByte = data.Split(seperator);
            foreach (var bb in strByte)
            {
                int intTemp;
                try
                {
                    intTemp = int.Parse(bb, NumberStyles.HexNumber);
                }
                catch (Exception)
                {
                    intTemp = 0;
                }
                listOfByte.Add((byte)intTemp);
            }
            return listOfByte.ToArray();

        }

        public static string ByteToString(byte[] asbyte)
        {
            if (asbyte.Length == 0) return "";
            var i = asbyte.Length >= 256 ? 256 : asbyte.Length;
            var strB = new StringBuilder(i * 3);

            for (var j = 0; j < i; j++)
                strB.AppendFormat("{0:X2} ", asbyte[j]);

            return strB.ToString();
        }
    }

    public class MessageFrame
    {
        public int Length { get; set; }
        public MessageCommand messageCommand { get; set; }
        public byte[] Data { get; set; }

        public MessageFrame(MessageCommand type, byte[] data)
        {
            messageCommand = type;
            Data = data;
            Length = data.Length;
        }

        public byte[] GetByte()
        {
            byte[] buffer = new byte[Length + 3];
            buffer[0] = (byte)messageCommand;

            byte[] lengthInBytes = BitConverter.GetBytes(Length);
            Array.Copy(lengthInBytes, 0, buffer, 1, 2);
            
            Array.Copy(Data, 0, buffer, 3, Length);

            return buffer;
        }
    }
}
