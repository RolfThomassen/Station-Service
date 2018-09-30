using System;
using System.Text;

namespace Storiveo.IsisPie.MessageTypes
{
    /*
     STX A Pump# Hose# Flag $$$$.$$ vvv.vvv ETX CD
    In the actual command, decimal points are implied only.
    Authorize Command Character=A
    Pump #=XX (01 to 32)
    Hose #=X (0 to 8, with 0 authorizing any hose — also see
    Multi-grade Lock Authorization which
    follows)
    Flag=X (Type of authorization — see Description)
    Dollar Limit Amount=$$$$.$$ (0000.01 to 9999.99)
    Volume Limit Amount=vvv.vvv (000.001 to 999.999)

    • Pump 1
    • Any Hose
    • $25.00
    STX A 0101002500000000 ETX CD
     */
    public class Authorise : ControlBase
    {
        private byte[] PumpIdAndAllNozzle, AuthoriseAmountAndVolume, Flag;

        public Authorise(int pumpId, decimal authoriseAmount)
        {
            PumpIdAndAllNozzle = Encoding.UTF8.GetBytes(string.Format("{0,2:F0}", pumpId).Replace(' ', '0') + 
                                                            "0");

            //authoriseAmount = Math.Round(authoriseAmount, 2);
            var tempAmount = authoriseAmount.ToString("F").Replace(".", "");

            //tempAmount = tempAmount + "00";
            if (tempAmount.Length > 6)
                tempAmount = "999999";
            AuthoriseAmountAndVolume = Encoding.UTF8.GetBytes(string.Format("{0,6:F0}", tempAmount).Replace(' ', '0') + 
                                                                 "000000");
            Flag = Encoding.UTF8.GetBytes("1");
        }

        public byte[] GetBytes()
        {
            int totalAuthoriseByte = 20;
            byte[] data = new byte[totalAuthoriseByte];

            data[0] = STX;
            data[1] = (byte)MessageCommand.Authorize;

            var currectIndex = 2;
            Array.Copy(PumpIdAndAllNozzle, 0, data, currectIndex, PumpIdAndAllNozzle.Length);
            currectIndex += PumpIdAndAllNozzle.Length;
            Array.Copy(Flag, 0, data, currectIndex, Flag.Length);
            currectIndex += Flag.Length;
            Array.Copy(AuthoriseAmountAndVolume, 0, data, currectIndex, AuthoriseAmountAndVolume.Length);

            data[totalAuthoriseByte - 2] = ETX;
            data[totalAuthoriseByte - 1] = GetCheckDigit(data);

            return data;
        }
    }
}
