namespace Storiveo.IsisPie.EventArgs
{
    public class AuthoriseStatusEventArgs : System.EventArgs
    {
        public int PumpId { get; set; }
        public string ZapOrderId { get; set; }
        public bool AuthoriseStatus { get; set; }

        public AuthoriseStatusEventArgs(int pumpId, string zapOrderId, bool authoriseStatus)
        {
            PumpId = pumpId;
            ZapOrderId = zapOrderId;
            AuthoriseStatus = authoriseStatus;
        }
    }
}