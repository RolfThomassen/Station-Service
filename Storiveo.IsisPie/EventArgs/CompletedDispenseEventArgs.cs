namespace Storiveo.IsisPie.EventArgs
{
    public class CompletedDispenseEventArgs : System.EventArgs
    {
        /*
         07H2M0$0000002000V0000009217P002170L002000f
        2017/10/25 23:34:39 Sale read Pump=7  Hose=2
        2017/10/25 23:34:39 Amount:00000020.00 
        2017/10/25 23:34:39 Volume:0000009.217 
        2017/10/25 23:34:39 PPU:002.170
        2017/10/25 23:34:39 Final sales data for pump 7, Amt=20.000000, Vol=9.217000, PPU=2.170000, Hose=2
        */

        public string ZapOrderId { get; set; }
        public int DispenserNo { get; set; }
        public int HoseNo { get; set; }
        public decimal DispensedAmount { get; set; }
        public decimal DispensedLiter { get; set; }
        public decimal Ppu { get; set; }

		public string BosTransactionId { get; set; }
        public string WebhookCompletedDispense { get; set; }      
		public string WebhookError { get; set; }
        
        private int DispenserNoLenght = 2;
        private int HoseNoLenght = 1;
        private int DispensedAmountLenght = 10;
        private int DispensedLiterLenght = 10;
        private int PpuLenght = 6;
        

		public CompletedDispenseEventArgs(string data, PiePumps piePumps, string bosTransactionId, 
		                                  string webhookCompletedDispense, string webhookError)
        {
            string tempStr = "";
            int dispenserNo = 0, hoseNo = 0;
            decimal dispensedAmount = 0, dispensedLiter = 0, ppu = 0;

            // 07H2M0$0000002000V0000009217P002170L002000f

            int.TryParse(data.Substring(data.IndexOf('H') - 2, DispenserNoLenght), out dispenserNo);

            int.TryParse(data.Substring(data.IndexOf('H') + 1, HoseNoLenght), out hoseNo);

            tempStr = data.Substring(data.IndexOf('$') + 1, DispensedAmountLenght);
            tempStr = tempStr.Insert(tempStr.Length - 2, ".");
            decimal.TryParse(tempStr, out dispensedAmount);

            tempStr = data.Substring(data.IndexOf('V') + 1, DispensedLiterLenght);
            tempStr = tempStr.Insert(tempStr.Length - 3, ".");
            decimal.TryParse(tempStr, out dispensedLiter);

            tempStr = data.Substring(data.IndexOf('P') + 1, PpuLenght);
            tempStr = tempStr.Insert(tempStr.Length - 3, ".");
            decimal.TryParse(tempStr, out ppu);

            /*
            if (data.Length > (DispenserNoLenght + HoseNoLenght + DispensedLiterLenght + DispensedAmountLenght + PpuLenght))
            {
                var index = 0;
                int.TryParse(data.Substring(index, DispenserNoLenght), out dispenserNo);
                index += DispenserNoLenght;
                int.TryParse(data.Substring(index, HoseNoLenght), out hoseNo);
                index += HoseNoLenght;
                decimal.TryParse(data.Substring(index, DispensedLiterLenght), out dispensedAmount);
                index += DispensedLiterLenght;
                decimal.TryParse(data.Substring(index, DispensedAmountLenght), out dispensedLiter);
                index += DispensedAmountLenght;
                decimal.TryParse(data.Substring(index, PpuLenght), out ppu);
            }*/

            DispenserNo = dispenserNo;
            HoseNo = hoseNo;
            DispensedAmount = dispensedAmount;
            DispensedLiter = dispensedLiter;
            Ppu = ppu;

            ZapOrderId = piePumps.Pumps[dispenserNo - 1].ZapOrderId;
			BosTransactionId = bosTransactionId;
			WebhookCompletedDispense = webhookCompletedDispense;
			WebhookError = webhookError;
        }
    }
}
