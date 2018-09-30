namespace Storiveo.IsisPie
{
    public class ControlBase
    {
        public byte STX { get; set; }
        public byte ETX { get; set; }

        public ControlBase()
        {
            STX = 0x02;
            ETX = 0x03;
        }

        protected byte GetCheckDigit(byte[] data)
        {
            byte cd = 0x00;
            for (int i = 0; i < data.Length - 1; i++)
            {
                cd += data[i];
            }
            return (byte)((0 - cd) & 0x7f);
        }
    }
}
