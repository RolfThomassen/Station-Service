using System;
using System.Text;

namespace Storiveo.IsisPie.MessageTypes
{
    /*
        Storiveo POS needs to send 'B07EHM$VPLb'  => 07 - PUMP no.
        Other all fields are same. B- Message identifier.
     */
    public class GetFinalization : ControlBase
    {
        private byte[] PumpId, OtherAllFields;

        public GetFinalization(int pumpId)
        {
            PumpId = Encoding.UTF8.GetBytes(string.Format("{0,2:F0}", pumpId).Replace(' ', '0'));
            OtherAllFields = Encoding.UTF8.GetBytes("EHM$VPL");
                                            
        }

        public byte[] GetBytes()
        {
            int totalAuthoriseByte = 13;
            byte[] data = new byte[totalAuthoriseByte];

            data[0] = STX;
            data[1] = (byte)MessageCommand.GetFinalization;

            var currectIndex = 2;
            Array.Copy(PumpId, 0, data, currectIndex, PumpId.Length);
            currectIndex += PumpId.Length;
            Array.Copy(OtherAllFields, 0, data, currectIndex, OtherAllFields.Length);

            data[totalAuthoriseByte - 2] = ETX;
            data[totalAuthoriseByte - 1] = GetCheckDigit(data);

            return data;
        }
    }
}
