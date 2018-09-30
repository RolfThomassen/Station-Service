namespace Storiveo.IsisPie.EventArgs
{
    public class PumpStatusEventArgs : System.EventArgs
    {
		public string RetailerId { get; set; }
		public int TotalOfPump { get; set; }
		public string PumpStatus { get; set; }

		public PumpStatusEventArgs(string retailerId, int totalOfPump, string pumpStatus)
        {
			RetailerId = retailerId;
			TotalOfPump = totalOfPump;
			PumpStatus = pumpStatus;
        }
    }
}