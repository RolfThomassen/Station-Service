
using System;
using System.Collections.Generic;

namespace Storiveo.IsisPie
{
    public class PumpPie
    {
        /*
         RequestTriedCount purpose
            - its like a timeout
            - once request is 0
            - will start counting when to receive a pump status
            - if more than 5 still the pump status is not changed to the request status
            - system will send to ZAP as request is failed
         */

        public string ZapOrderId { get; set; }
        public int PumpId { get; set; }
        public RequestPumpStatus Request { get; set; }
        public int RequestTriedCount { get; set; }
        public PumpStatus CurrentStatus { get; set; }        
		public bool NotifyDispenseStarted { get; set; }
        public RequestPumpStatus PreviousRequest { get; set; }
		public int CompletedDispenseCount { get; set; }
		public bool UnexectedPumpDispenseCompletedFlag { get; set; }
        
		public string BosTransactionId { get; set; }
        public string WebhookStartDispense { get; set; }
        public string WebhookCompletedDispense { get; set; }
        public string WebhookError { get; set; }
    }

    public class PiePumps
    {
        public List<PumpPie> Pumps = new List<PumpPie>();
        public int TotalOfPump = 14;

        public PiePumps()
        {
            createAllPump();
        }

        private void createAllPump()
        {
            Pumps.Clear();

            for (int i = 1; i <= TotalOfPump; i++)
            {
                var newPump = new PumpPie
                {
                    PumpId = i,
                    Request = RequestPumpStatus.IdleNoRequest,
                    RequestTriedCount = 0,
                    CurrentStatus = PumpStatus.Idle,
					BosTransactionId = "",
					WebhookStartDispense = "",
					WebhookCompletedDispense = "",
					WebhookError = ""
                };
                Pumps.Add(newPump);
            }
        }

        public void pumpStatusUpdate(string statusFromPie)
        {
            int currentIndex = 0;
            try
            {
                for (int i = 0; i < TotalOfPump; i++)
                {
                    // 28 02 12 02 0: 12 12 02 12 02 02 0000000000000000000000000000000000000000000
                    // 280212020:1212021202020000000000000000000000000000000000000000000

                    if (i == 0)
                        currentIndex = 3;

                    var theStatus = PumpStatus.DoNothing;
                    var status = statusFromPie.Substring(currentIndex, 2);
                    currentIndex += 2;

                    switch (status)
                    {
                        case "02":
                            theStatus = PumpStatus.Idle;
                            break;
                            // No reserve. Pie did not support for reserve command
                        case "0#":      // please confirm it is correct
                            theStatus = PumpStatus.Reserved;
                            break;
                        case "0;":
                            theStatus = PumpStatus.StartedDispend;
                            break;
                        case "0?":
                        case "1?":
                        case "2?":
                            theStatus = PumpStatus.Dispensing;
							break;
                        case "92":
                        case "12":
                            theStatus = PumpStatus.DispenseCompleted;
                            break;
                        case "0:":
                        case "1:":
                            theStatus = PumpStatus.Authorized;
                            break;
                        case "00":
                            theStatus = PumpStatus.NoRespond;
                            break;
                        case "03":
                        case "13":
                        case "23":
                            theStatus = PumpStatus.NoAuthorise;
                            break;
                        case "82":
                            theStatus = PumpStatus.PumpBlocked;
                            break;
                    }

                    Pumps[i].CurrentStatus = theStatus;
                }
            }
            catch (Exception ex)
            {
                
            }

        }
    }

}
