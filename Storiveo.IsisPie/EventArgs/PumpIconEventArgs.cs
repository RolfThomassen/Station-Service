using System.Drawing;

namespace Storiveo.IsisPie.EventArgs
{
    public class PumpIconEventArgs : System.EventArgs
    {
        public PumpIconEventArgs(int pumpNumber, string fuelStatus, Color fuelStatusColor,
                                 string clientReference, string clientActivity)
        {
            PumpNumber = pumpNumber;
            FuelStatus = fuelStatus;
            FuelStatusColor = fuelStatusColor;
            ClientActivity = "ZAP";
            ClientReference = clientReference;
        }

        //public PumpIconEventArgs(int pumpNumber, string pumpImage)
        //{
        //    PumpNumber = pumpNumber;
        //    PumpImage = pumpImage;
        //}

        public int PumpNumber { get; set; }
        public string PumpImage { get; set; }

        public Color FuelStatusColor { get; set; }
        public string FuelStatus { get; set; }
        public string ClientReference { get; set; }
        public string ClientActivity { get; set; }
    }
}