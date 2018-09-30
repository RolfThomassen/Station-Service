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
    public class Reserve : ControlBase
    {
        private byte[] PumpId;
        private byte[] ZapId;

        int ZapIdLenght = 13;

        public Reserve(int pumpId, string zapId)
        {
            PumpId = Encoding.UTF8.GetBytes(string.Format("{0,2:F0}", pumpId).Replace(' ', '0'));

            if (zapId.Length > ZapIdLenght)
                zapId = zapId.Substring(zapId.Length - ZapIdLenght, ZapIdLenght);
            
            ZapId = Encoding.UTF8.GetBytes(string.Format("{0," + ZapIdLenght + ":F0}", zapId));
        }

        public byte[] GetBytes()
        {
            int totalAuthoriseByte = 6 + ZapIdLenght;
           
            byte[] data = new byte[totalAuthoriseByte];

            data[0] = STX;
            data[1] = (byte)MessageCommand.Reserve;

            var currectIndex = 2;
            Array.Copy(PumpId, 0, data, currectIndex, PumpId.Length);
            currectIndex += PumpId.Length;
            Array.Copy(ZapId, 0, data, currectIndex, ZapIdLenght);

            data[totalAuthoriseByte - 2] = ETX;
            data[totalAuthoriseByte - 1] = GetCheckDigit(data);

            return data;
        }
    }
}
