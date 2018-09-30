using System;
using System.Text;

namespace Storiveo.IsisPie.MessageTypes
{
    /*
    'Mxx' 
    M- Message identifier
    xx- Pump No
    STX M xx ETX CD
     */
    public class CancelPump : ControlBase
    {
        private byte[] PumpId;

        public CancelPump(int pumpId)
        {
            PumpId = Encoding.UTF8.GetBytes(string.Format("{0,2:F0}", pumpId).Replace(' ', '0'));
        }

        public byte[] GetBytes()
        {
            int totalAuthoriseByte = 7;
            byte[] data = new byte[totalAuthoriseByte];

            data[0] = STX;
            data[1] = (byte)MessageCommand.PumpCancel;

            var currectIndex = 2;
            Array.Copy(PumpId, 0, data, currectIndex, PumpId.Length);

            data[4] = (byte)MessageCommand.PumpCancel2;

            data[totalAuthoriseByte - 2] = ETX;
            data[totalAuthoriseByte - 1] = GetCheckDigit(data);

            return data;
        }
    }
}
