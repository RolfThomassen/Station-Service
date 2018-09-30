namespace Storiveo.IsisPie.EventArgs
{
    public class StartedDispenseEventArgs : System.EventArgs
    {
        public int PumpId { get; set; }
        public string ZapOrderId { get; set; }

        public string BosTransactionId { get; set; }
        public string WebhookStartDispense { get; set; }

		public StartedDispenseEventArgs(int pumpId, string zapOrderId, 
		                                string bosTransactionId, string webhookStartDispense)
        {
            PumpId = pumpId;
            ZapOrderId = zapOrderId;
			BosTransactionId = bosTransactionId;
			WebhookStartDispense = webhookStartDispense;
        }
    }
}
