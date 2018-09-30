using System;
using System.Text;

namespace Storiveo.IsisPie.MessageTypes
{
    public class PumpStatusRequest : ControlBase
    {
        byte[] Command;

        public PumpStatusRequest()
        {
            Command = Encoding.UTF8.GetBytes("FE");
        }
        public byte[] GetBytes()
        {
            //_msg.MessageOffset();
            byte[] data = new byte[5];
            data[0] = STX;
            Array.Copy(Command, 0, data, 1, 2);
            data[3] = ETX;
            data[4] = GetCheckDigit(data);
            return data;
        }
    }
}
