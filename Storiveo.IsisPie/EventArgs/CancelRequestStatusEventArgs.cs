namespace Storiveo.IsisPie.EventArgs
{
    public class CancelRequestStatusEventArgs : System.EventArgs
    {
        public int PumpId { get; set; }
        public string ZapOrderId { get; set; }
        public bool CancelStatus { get; set; }

        public CancelRequestStatusEventArgs(int pumpId, string zapOrderId, bool cancelStatus)
        {
            PumpId = pumpId;
            ZapOrderId = zapOrderId;
            CancelStatus = cancelStatus;
        }
    }
}