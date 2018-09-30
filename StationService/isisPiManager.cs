using System;
using System.Diagnostics;
using System.Text;
using Storiveo.IsisPie;
using Storiveo.IsisPie.EventArgs;

namespace StationServices
{
    public class isisPiManager
    {
		private IsisPie _isisPie;

		public isisPiManager()
        {
			_isisPie = new IsisPie();

			_isisPie.OnAuthoriseStatus += _isisPie_OnAuthoriseStatus;
            _isisPie.OnCompletedDispense += _isisPie_OnCompletedDispense;
            _isisPie.OnReserveStatus += _isisPie_OnReserveStatus;
            _isisPie.OnStartedDispense += _isisPie_OnStartedDispense;
            _isisPie.OnCancelRequestStatus += _isisPie_OnCancelRequestStatus;
			_isisPie.OnPumpStatus += _isisPie_OnPumpStatus;
			_isisPie.OnSendToPumpController += _isisPie_OnSendToPumpController;
			//_isisPie.OnPumpIconChanged += _isisPie_OnPumpIconChanged;
            
        }

		public async void RequestPumpReserve(string zapOrderId, int pumpId)
		{
			// zapOrderId and PumpId shold be saved in Database/Memory to be referred later
			await _isisPie.SendDataToPie(MessageCommand.Reserve, zapOrderId, pumpId);
		}


		public async void RequestPumpAuthorise(string zapOrderId, string transactionId, decimal authoriseLimit,
                                       string webhook_start, string webhook_success, string webhook_error)
        {
			// from the zapOrderId, should get the Pump Id from Database/Memory using zapOrderId
			var pumpId = 1;
			await _isisPie.SendDataToPie(MessageCommand.Authorize, zapOrderId, pumpId, authoriseLimit, 
			                             transactionId, webhook_start, webhook_success, webhook_error);
        }
        
		public async void RequestPumpCancel(string zapOrderId, int pumpId)
        {
			await _isisPie.SendDataToPie(MessageCommand.PumpCancel2, zapOrderId, pumpId);
        }


		//
        // Arrived message from RabbitMq Handler
        //

		public async void messageReceivedFromRabbitMQProcess(byte[] data)
		{
			await _isisPie.ProcessDataReceived(data);       
		}

        //
        // Events Trigger Handler
        //

        private void _isisPie_OnSendToPumpController(object sender, SendToPumpControllerEventArgs e)
        {
			// e.Data byte to be send to RabbitMQ 
			Debug.WriteLine(Encoding.UTF8.GetString(e.Data, 0, e.Data.Length));
			                
			if (e.Data.Length > 6)
			{
				// not the pump status request
			}

        }

        private void _isisPie_OnPumpStatus(object sender, PumpStatusEventArgs e)
        {
			// Contents of this function have to send to BackOffice Cloud 
			Debug.WriteLine("Send to BO Cloud: Pump Status");
        }

        private void _isisPie_OnCancelRequestStatus(object sender, CancelRequestStatusEventArgs e)
        {
			// Contents of this function have to send to BackOffice Cloud 
			Debug.WriteLine("Send to BO Cloud: Cancel Request Status Pump " + e.PumpId + ". Status: " + e.CancelStatus);
            
        }

        private void _isisPie_OnStartedDispense(object sender, StartedDispenseEventArgs e)
        {
			// Contents of this function have to send to BackOffice Cloud 
			Debug.WriteLine("Send to BO Cloud: Pump " + e.PumpId + " Started Dispense");
        }

        private void _isisPie_OnReserveStatus(object sender, ReserveStatusEventArgs e)
        {
			// Contents of this function have to send to BackOffice Cloud 
			Debug.WriteLine("Send to BO Cloud: Reserve Status Pump " + e.PumpId + ". Status: " + e.ReserveStatus);
        }

        private void _isisPie_OnPumpIconChanged(object sender, PumpIconEventArgs e)
        {
			// Contents of this function have to send to BackOffice Cloud 
        }

        private void _isisPie_OnCompletedDispense(object sender, CompletedDispenseEventArgs e)
        {
			// Contents of this function have to send to BackOffice Cloud 
			Debug.WriteLine("Send to BO Cloud: Completed Dispense Pump " + e.HoseNo);
        }

        private void _isisPie_OnAuthoriseStatus(object sender, AuthoriseStatusEventArgs e)
        {
			// Contents of this function have to send to BackOffice Cloud 
			Debug.WriteLine("Send to BO Cloud: Authorise Status Pump " + e.PumpId + ". Status: " + e.AuthoriseStatus);
        }

    }
}
