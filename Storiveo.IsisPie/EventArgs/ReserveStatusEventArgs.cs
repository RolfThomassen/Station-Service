namespace Storiveo.IsisPie.EventArgs
{
    public class ReserveStatusEventArgs : System.EventArgs
    {
        public int PumpId { get; set; }
        public string ZapOrderId { get; set; }
        public bool ReserveStatus { get; set; }

		public ReserveStatusEventArgs(int pumpId, string zapOrderId, bool reserveStatus)
        {
            PumpId = pumpId;
            ZapOrderId = zapOrderId;
			ReserveStatus = reserveStatus;
        }
    }
}