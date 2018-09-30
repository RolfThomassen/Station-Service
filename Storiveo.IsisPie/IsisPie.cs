using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Storiveo.IsisPie.EventArgs;
using Storiveo.IsisPie.MessageTypes;
using System.Drawing;

namespace Storiveo.IsisPie
{
	public class IsisPie
	{
		bool DEBUG_MODE = true;         // most important debug mode
		bool DEBUG_MODE2 = false;        // second important

		private Task threadTcpListening;
		private Timer tmrBankServerConnectionCheck;

		MessagesBase msg = new MessagesBase();
		public StorageAccess StorageAccess;

		public event EventHandler<ReserveStatusEventArgs> OnReserveStatus;
		public event EventHandler<AuthoriseStatusEventArgs> OnAuthoriseStatus;
		public event EventHandler<StartedDispenseEventArgs> OnStartedDispense;
		public event EventHandler<CompletedDispenseEventArgs> OnCompletedDispense;
		public event EventHandler<PumpIconEventArgs> OnPumpIconChanged;
		public event EventHandler<CancelRequestStatusEventArgs> OnCancelRequestStatus;
		public event EventHandler<PumpStatusEventArgs> OnPumpStatus;
		public event EventHandler<SendToPumpControllerEventArgs> OnSendToPumpController;

		public bool BankServerConnected = false;
		private string previousMessageStatus = "";

		PiePumps PiePumps = new PiePumps();

		//msg.MessageOffset();
		#region Declaration
		public IsisPie()
		{
			StorageAccess = new StorageAccess(ref msg);
			BankServerInit();
			tmrBankServerConnectionCheck = new Timer(BankServerConnectionCheck, true, 1000, 1000);
		}

		private void BankServerConnectionCheck(object state)
		{
			try
			{
				//var testIt = TcpClnt.RemotePort;    // will throw an error when is not connected

				SendDataToPie(MessageCommand.RequestPumpStatus, "");
			}
			catch (Exception e)
			{
				updatePumpStatus("");
				Debug.WriteLine(e);
				BankServerInit();
			}
		}

		private async void BankServerInit()
		{
			var port = SystemParameters.BankServerPort;
			var host = SystemParameters.BankServerIp;

			//  port = "6220";
			//  host = "198.54.233.51";
			// host = "198.54.233.102";

			try
			{
				BankServerConnected = false;

				// TcpClnt = new TcpSocketClient();
				// await TcpClnt.ConnectAsync(host, int.Parse(port));

				threadTcpListening = Task.Factory.StartNew(ThreadingTcpListen);

				Debug.WriteLine("Connected to " + host);
				BankServerConnected = true;

				//await updatePumpStatusFromDb();
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Failed to connect. Error:" + ex.Message);
			}
		}

		#region BankServer

		public async void ResetConnection()
		{
			//await TcpClnt.DisconnectAsync();
			await Task.Delay(1000);
			//  CheckConnection(null, null);
		}

		//private async void ThreadingTcpListen()
		public async Task ThreadingTcpListen()
		{
			try
			{
				/*
				int counter = 0;
				while (true)
				{
					if (TcpClnt != null)
					{
						var bytesReceived = new byte[200];
						await TcpClnt.ReadStream.ReadAsync(bytesReceived, 0, bytesReceived.Length);
						await ProcessDataReceived(bytesReceived);
						//}
						await Task.Delay(200);
					}
					else
					{
						if (counter == 5)
							break;
						counter++;
						await Task.Delay(300);
					}
				}
				*/
			}
			catch (Exception ex)
			{
				ResetConnection();
				Debug.WriteLine("ThreadingTcpListen. Error:" + ex.Message);
			}
		}

		#endregion

		private async Task updatePumpStatusFromDb()
		{
			// now is not required due to 
			/*
			var pumpCurrentRequest = await StorageAccess.GetPumpRequestStatus();
			for (int i = 0; i < pumpCurrentRequest.Count; i++)
			{
				if (pumpCurrentRequest[i].AuthoriseDateTime == DateTime.MinValue)
					continue;

				if (pumpCurrentRequest[i].CompletedDateTime == DateTime.MinValue)
				{
					PiePumps.Pumps[pumpCurrentRequest[i].PumpId - 1].Request = RequestPumpStatus.RequestDispenseCompleted;
					PiePumps.Pumps[pumpCurrentRequest[i].PumpId - 1].ZapOrderId = pumpCurrentRequest[i].OrderId;
     
					var status = "|" + PiePumps.Pumps[i].PumpId + "*" +
                                                 PiePumps.Pumps[i].ZapOrderId + "*" +
                                                 PiePumps.Pumps[i].Request.ToString("G") + "*" +
                                                 PiePumps.Pumps[i].CurrentStatus.ToString("G") + "*" +
                                                 PiePumps.Pumps[i].RequestTriedCount + "|";
                    Debug.WriteLine(status);
                    

				}
			}
			*/
		}

		public async Task SendDataToPie(MessageCommand messageCommand, string ZapOrderId,
										int pumpId = 0, decimal authoriseLimit = 0,
										string transactionId = "", string webhook_start = "",
										string webhook_success = "", string webhook_error = "")
		{
			var dataByte = new byte[1];
			byte[] buffer = dataByte;
			string dataBuffer;

			try
			{
				switch (messageCommand)
				{
					case MessageCommand.RequestPumpStatus:
						//return;
						buffer = new PumpStatusRequest().GetBytes();
						break;
					case MessageCommand.Reserve:
						if (PiePumps.Pumps[pumpId - 1].CurrentStatus == PumpStatus.Idle)
						{
							PiePumps.Pumps[pumpId - 1].Request = RequestPumpStatus.RequestReserve;
							PiePumps.Pumps[pumpId - 1].ZapOrderId = ZapOrderId;
							PiePumps.Pumps[pumpId - 1].RequestTriedCount = 0;

							PiePumps.Pumps[pumpId].UnexectedPumpDispenseCompletedFlag = false;

							buffer = new Reserve(pumpId, ZapOrderId).GetBytes();
						}
						else
						{
							// straight away to reject it
							ReservePump(pumpId, false);
						}
						break;
					case MessageCommand.Authorize:
						if (PiePumps.Pumps[pumpId - 1].Request != RequestPumpStatus.RequestAutorise)
						{
							Debug.WriteLine("Failed. Pump is not in RequestAutorise state");
							AuthorizePump(pumpId, false);
							break;
						}

						PiePumps.Pumps[pumpId - 1].RequestTriedCount = 0;
						PiePumps.Pumps[pumpId - 1].BosTransactionId = transactionId;
						PiePumps.Pumps[pumpId - 1].WebhookStartDispense = webhook_start;
						PiePumps.Pumps[pumpId - 1].WebhookCompletedDispense = webhook_success;
						PiePumps.Pumps[pumpId - 1].WebhookError = webhook_error;

						buffer = new Authorise(pumpId, authoriseLimit).GetBytes();
						break;
					case MessageCommand.GetFinalization:
						PiePumps.Pumps[pumpId - 1].Request = RequestPumpStatus.RequestFinalize;
						PiePumps.Pumps[pumpId - 1].RequestTriedCount = 0;

						buffer = new GetFinalization(pumpId).GetBytes();
						break;
					case MessageCommand.PumpCancel2:
                        PiePumps.Pumps[pumpId - 1].PreviousRequest = PiePumps.Pumps[pumpId - 1].Request;    // probably cancel request is failed

						if (PiePumps.Pumps[pumpId - 1].CurrentStatus == PumpStatus.Reserved ||
						    PiePumps.Pumps[pumpId - 1].CurrentStatus == PumpStatus.Authorized)
						{
							bool okProceed = true;
							if (ZapOrderId != "")
							{
							    if (PiePumps.Pumps[pumpId - 1].ZapOrderId != ZapOrderId)        // make sure the same order id)
								{
									okProceed = false;
								}
							}

							if (okProceed)
							{
								PiePumps.Pumps[pumpId - 1].RequestTriedCount = 0;
								PiePumps.Pumps[pumpId - 1].Request = RequestPumpStatus.RequestCancel;
								buffer = new CancelPump(pumpId).GetBytes();

								break;
							}
						}
      
						PiePumps.Pumps[pumpId - 1].RequestTriedCount = 9999;       // terus cancel
						processCancel(pumpId - 1, ZapOrderId);
						return;
				}

				if (messageCommand != MessageCommand.RequestPumpStatus)
				{
					dataBuffer = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
					SystemLogs.LogBankServer(dataBuffer, "POS", messageCommand.ToString("G"));
				}
				await Write(buffer);

				if (DEBUG_MODE)
				{
					if (messageCommand != MessageCommand.RequestPumpStatus)
					{
						Debug.WriteLine("--------> Request to Pie for " + messageCommand.ToString("G") + " Pump No:" + pumpId +
										" Data:" + Encoding.UTF8.GetString(buffer, 0, buffer.Length));
					}
				}

			}
			catch (Exception ex)
			{
				dataBuffer = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
				SystemLogs.LogError("SendDataToBankServer Exception", "Error Message: " + ex.Message,
					"Buffer:" + dataBuffer);
			}

		}

		public async Task ProcessDataReceived(byte[] data)
		{
			string dataInString = "INIT";
			try
			{
				dataInString = Encoding.UTF8.GetString(data, 0, data.Length);
				if (dataInString.Trim() == "")
					return;

				MessageType messageType = MessageType.Unknown;

				if (dataInString.Contains("$"))
				{
					messageType = MessageType.AuthorizationInfo;
				}
				else
				{
					string messageTypeIdentifier = Encoding.UTF8.GetString(data, 1, 1);
					Enum.TryParse<MessageType>(messageTypeIdentifier, out messageType);
				}

				if (messageType == MessageType.PumpStatus)
				{
					int minPumpStatusData = 30;
					if (data.Length > minPumpStatusData)
					{
						dataInString = dataInString.Substring(0, 68);
						await updatePumpStatus(dataInString);

						for (int i = 0; i < PiePumps.TotalOfPump; i++)
						{
							if (PiePumps.Pumps[i].CurrentStatus == PumpStatus.DispenseCompleted)
							{
								if (PiePumps.Pumps[i].Request != RequestPumpStatus.RequestDispenseCompleted)
								{
									await processUnexectedPumpDispenseCompleted(i);
									continue;
								}
							}
							else
							{
								if (PiePumps.Pumps[i].Request == RequestPumpStatus.RequestReserve)
								{
									processReserve(i);
									continue;
								}
								else
								if (PiePumps.Pumps[i].Request == RequestPumpStatus.RequestAutorise)
								{
									processAuthorise(i);
									continue;
								}
								else if (PiePumps.Pumps[i].Request == RequestPumpStatus.RequestStartedDispense)
								{
									processRunning(i);
									continue;
								}
								else if (PiePumps.Pumps[i].Request == RequestPumpStatus.RequestDispenseCompleted)
								{
									await processDispenseCompleted(i);
									continue;
								}
								else if (PiePumps.Pumps[i].Request == RequestPumpStatus.RequestCancel)
								{
									processCancel(i, PiePumps.Pumps[i].ZapOrderId);
									continue;
								}
							}
						}

						if (DEBUG_MODE)
						{
							for (int i = 0; i < PiePumps.TotalOfPump; i++)
							{
								//if (i > 2)
									//return;

								if (PiePumps.Pumps[i].Request > (RequestPumpStatus)2)
								{
									var status = "|" + PiePumps.Pumps[i].PumpId + "*" +
														 PiePumps.Pumps[i].ZapOrderId + "*" +
														 PiePumps.Pumps[i].Request.ToString("G") + "*" +
														 PiePumps.Pumps[i].CurrentStatus.ToString("G") + "*" +
									                     PiePumps.Pumps[i].RequestTriedCount + "|";
									Debug.WriteLine(status);
								}
							}
						}
					}
				}
				else if (messageType == MessageType.AuthorizationInfo)
				{
					GetAuthorization(dataInString);
					SystemLogs.LogBankServer(dataInString + 1, "POS", "Dispense Info");
				}
				//else if (messageType == MessageType.ReserveStatus)
				//{
				//    GetReserveStatus(dataInString);
				//}
				else
				// Sometime it is not trigger properly the reserve status
				// below is the workaround
				if (Encoding.UTF8.GetString(data, 1, 1) == "M")
				{
					GetReserveStatus(dataInString);
				}

				// Log every received msj to DB. BS is BankServer
				// SystemLogs.LogBankServer(dataInString, "PI", messageType.ToString("G"));

				if (DEBUG_MODE)
				{
					if (messageType != MessageType.PumpStatus)
					{
						Debug.WriteLine("Received from Pie: " + dataInString + Environment.NewLine);
					}
					else
					{
						if (DEBUG_MODE2)
						{
							Debug.WriteLine("Received from Pie: " + dataInString + Environment.NewLine);
						}
					}
				}

			}
			catch (Exception ex)
			{
				SystemLogs.LogError("ProcessDataReceived Exception", "Error Message: " + ex.Message,
									"Received Data: " + dataInString);
				Debug.WriteLineIf(ex.Message != "", ex.Message);
			}
		}

		private async Task processUnexectedPumpDispenseCompleted(int pumpId)
        {
			if (PiePumps.Pumps[pumpId].UnexectedPumpDispenseCompletedFlag)
				return;
			
            PiePumps.Pumps[pumpId].CompletedDispenseCount += 1;
            if (PiePumps.Pumps[pumpId].CompletedDispenseCount > 5)      // 5 seconds wait is enought to make sure the 
            {                                                           // pump transaction is not own by others POS
				PiePumps.Pumps[pumpId].UnexectedPumpDispenseCompletedFlag = true;
                PiePumps.Pumps[pumpId].CompletedDispenseCount = 0;

				// await updatePumpStatusFromDb();

                // straitghaway to clear it. no need to check in DB
				await processDispenseCompleted(pumpId);

                if (DEBUG_MODE)
                {
					Debug.WriteLine("-> processUnexectedPumpDispenseCompleted for Pump Number " + pumpId);
					SystemLogs.LogBankServer("Pump No" + pumpId + 1, "POS", "processUnexectedPumpDispenseCompleted");
                }
            }
			else if (PiePumps.Pumps[pumpId].CompletedDispenseCount > 15)      // 15 seconds wait is enought to make sure the 
			{
				// retry when the previous requst is failed
				PiePumps.Pumps[pumpId].UnexectedPumpDispenseCompletedFlag = false;
				PiePumps.Pumps[pumpId].CompletedDispenseCount = 0;
				if (DEBUG_MODE)
                {
                    Debug.WriteLine("-> processUnexectedPumpDispenseCompleted for Pump Number " + pumpId);
                    SystemLogs.LogBankServer("Pump No" + pumpId + 1, "POS", "processUnexectedPumpDispenseCompleted Retry");
                }
			}
        }      

		private void GetReserveStatus(string dataInString)
		{
			int DispenserNoLenght = 2;
			int statusLenght = 1;

			int pumpId = 0;
			int status = 0;

			bool reserveStatus = false;

			var index = 2;
			int.TryParse(dataInString.Substring(index, DispenserNoLenght), out pumpId);
			index += DispenserNoLenght;
			int.TryParse(dataInString.Substring(index, statusLenght), out status);

			pumpId--;

			if (status == 0)
			{
				return;     // not required due to it is handled by pump status

				reserveStatus = true;
				PiePumps.Pumps[pumpId].Request = RequestPumpStatus.RequestAutorise;
			}
			else
			{
				PiePumps.Pumps[pumpId].Request = RequestPumpStatus.IdleNoRequest;
			}

			ReservePump(pumpId, reserveStatus);
		}

		private void GetAuthorization(string dataInString)
		{         
			int.TryParse(dataInString.Substring(dataInString.IndexOf('H') - 2, 2), out var pumpId);
			SendDataToPie(MessageCommand.PumpCancel2, "", pumpId);

			PiePumps.Pumps[pumpId - 1].Request = RequestPumpStatus.IdleNoRequest;


            OnCompletedDispense(this, new CompletedDispenseEventArgs(dataInString, PiePumps,
			                                                         PiePumps.Pumps[pumpId - 1].BosTransactionId,
			                                                         PiePumps.Pumps[pumpId - 1].WebhookCompletedDispense,
			                                                         PiePumps.Pumps[pumpId - 1].WebhookError));

			if (DEBUG_MODE)
			{
				Debug.WriteLine("-> GetAuthorization is Process ");
			}
			//return null;
		}

		private void processReserve(int pumpId)
		{
			PiePumps.Pumps[pumpId].RequestTriedCount += 1;

			//if (PiePumps.Pumps[pumpId].RequestTriedCount > (SystemParameters.reserveTimout / 2))
			if (PiePumps.Pumps[pumpId].RequestTriedCount > SystemParameters.reserveTimout)
			{
				// This is timeOut Event            
				PiePumps.Pumps[pumpId].Request = RequestPumpStatus.IdleNoRequest;
				ReservePump(pumpId, false);

				if (DEBUG_MODE)
				{
					Debug.WriteLine("-> processReserve for Pump Number " + pumpId + " TimeOUT in Storiveo");
				}
			}
			else if (PiePumps.Pumps[pumpId].CurrentStatus == PumpStatus.Reserved)
			{
				// This is successfully event            
				PiePumps.Pumps[pumpId].Request = RequestPumpStatus.RequestAutorise;
				PiePumps.Pumps[pumpId].NotifyDispenseStarted = false;
				ReservePump(pumpId, true);

				Task.Delay(1000);   // wait a bit to allow ISIS completed do his task before receive a new request

				if (DEBUG_MODE)
				{
					Debug.WriteLine("-> processReserve for Pump Number " + pumpId + " SuccessFully");
				}}
		}

		private void ReservePump(int pumpId, bool ReserveStatus)
		{
			OnReserveStatus(this, new ReserveStatusEventArgs(
				PiePumps.Pumps[pumpId].PumpId,
				PiePumps.Pumps[pumpId].ZapOrderId,
				ReserveStatus));

			PiePumps.Pumps[pumpId].RequestTriedCount = 0;
		}

		private void processAuthorise(int pumpId)
		{
			PiePumps.Pumps[pumpId].RequestTriedCount += 1;

			//if (PiePumps.Pumps[pumpId].RequestTriedCount > (SystemParameters.reserveTimout / 2))
			if (PiePumps.Pumps[pumpId].RequestTriedCount > SystemParameters.reserveTimout)
			{
				// send to ZAP that the pump has been successfully failed due to timeout
				SendDataToPie(MessageCommand.PumpCancel2, "", PiePumps.Pumps[pumpId].PumpId);

				PiePumps.Pumps[pumpId].Request = RequestPumpStatus.IdleNoRequest;
				ReservePump(pumpId, false);     // under reserve mode

				if (DEBUG_MODE)
				{
					Debug.WriteLine("-> processAuthorise for Pump Number " + pumpId + " TimeOut in Storiveo");
				}

			}
			else
			if (PiePumps.Pumps[pumpId].CurrentStatus == PumpStatus.Authorized)
			{
				// send to ZAP that the pump has been successfully reserved
				PiePumps.Pumps[pumpId].Request = RequestPumpStatus.RequestStartedDispense;
				AuthorizePump(pumpId, true);

				if (DEBUG_MODE)
				{
					Debug.WriteLine("-> processAuthorise for Pump Number " + pumpId + " SuccessFully");
				}

			}
			else if (PiePumps.Pumps[pumpId].CurrentStatus == PumpStatus.Idle)
			{
				// This is TimeOut Event in ISIS POS            
				PiePumps.Pumps[pumpId].Request = RequestPumpStatus.IdleNoRequest;
				ReservePump(pumpId, false);     // under reserve mode

				if (DEBUG_MODE)
				{
					Debug.WriteLine("-> processAuthorise for Pump Number " + pumpId + " TimeOut in POS");
				}
			}
		}

		private void AuthorizePump(int pumpId, bool AuthoriseStatus)
		{
			OnAuthoriseStatus(this, new AuthoriseStatusEventArgs(
				PiePumps.Pumps[pumpId].PumpId,
				PiePumps.Pumps[pumpId].ZapOrderId,
					AuthoriseStatus));

			PiePumps.Pumps[pumpId].RequestTriedCount = 0;
		}

		private void processRunning(int pumpId)
		{
			PiePumps.Pumps[pumpId].RequestTriedCount += 1;
			if (PiePumps.Pumps[pumpId].RequestTriedCount > SystemParameters.authoriseTimout)
			{
				// send to ZAP that the pump has been successfully failed due to timeout
				SendDataToPie(MessageCommand.PumpCancel2, "", PiePumps.Pumps[pumpId].PumpId);

				PiePumps.Pumps[pumpId].Request = RequestPumpStatus.IdleNoRequest;            
				AuthorizePump(pumpId, false);

				if (DEBUG_MODE)
				{
					Debug.WriteLine("-> processAuthorise for Pump Number " + pumpId + " TimeOut in Storiveo");
				}

			}
			else if (PiePumps.Pumps[pumpId].CurrentStatus == PumpStatus.Dispensing)
			{
				//if (PiePumps.Pumps[pumpId].Request != RequestPumpStatus.RequestDispenseCompleted)
				if (PiePumps.Pumps[pumpId].NotifyDispenseStarted == false)
				{               
                    PiePumps.Pumps[pumpId].NotifyDispenseStarted = true;

					OnStartedDispense(this, new StartedDispenseEventArgs(
						PiePumps.Pumps[pumpId].PumpId,
						PiePumps.Pumps[pumpId].ZapOrderId,
						PiePumps.Pumps[pumpId].BosTransactionId,
						PiePumps.Pumps[pumpId].WebhookStartDispense));
				}

				PiePumps.Pumps[pumpId].Request = RequestPumpStatus.RequestDispenseCompleted;
				if (DEBUG_MODE)
                {
                    Debug.WriteLine("-> processRunning for Pump Number " + pumpId + " Success");
                }
			}
			else if (PiePumps.Pumps[pumpId].CurrentStatus == PumpStatus.Idle)
			{
				// This is cancel event from ISIS POS
				// Hose No 1 is selected as default
				PiePumps.Pumps[pumpId].Request = RequestPumpStatus.IdleNoRequest;
				string dummyAuthotizationInfo = "0" + PiePumps.Pumps[pumpId].PumpId + "H1M0$0000000000V0000000000P000000L00000";
				GetAuthorization(dummyAuthotizationInfo);

				if (DEBUG_MODE)
				{
					Debug.WriteLine("-> processRunning for Pump Number " + pumpId + " ISIS POS Cancel it");
				}
			}
		}

		private async Task processDispenseCompleted(int pumpId)
		{
			if (PiePumps.Pumps[pumpId].CurrentStatus == PumpStatus.DispenseCompleted)
			{

				PiePumps.Pumps[pumpId].Request = RequestPumpStatus.RequestFinalize;

				await SendDataToPie(MessageCommand.GetFinalization, PiePumps.Pumps[pumpId].ZapOrderId,
									PiePumps.Pumps[pumpId].PumpId);

				await Task.Delay(1000);

				if (DEBUG_MODE)
				{
					Debug.WriteLine("-> processDispenseCompleted for Pump Number " + pumpId + "Success");
				}
			}
			else if (PiePumps.Pumps[pumpId].CurrentStatus == PumpStatus.Authorized)
			{
			    // This is Nozzle picked up but never dispense
                // Them correction on pump request status to be execute
				PiePumps.Pumps[pumpId].Request = RequestPumpStatus.RequestStartedDispense;
                
			    if (DEBUG_MODE)
			    {
			        Debug.WriteLine("-> processDispenseCompleted but Pump In Ready Mode for Pump Number " + pumpId + "Success");
			    }
			}
		}


		private void processCancel(int pumpId, string requestZapOrderId)
		{
			if (requestZapOrderId == "")
				return;

			PiePumps.Pumps[pumpId].RequestTriedCount += 1;
			if (PiePumps.Pumps[pumpId].RequestTriedCount > SystemParameters.cancelTimout)
			{
				// send to ZAP that the pump has been successfully failed due to timeout
				PiePumps.Pumps[pumpId].Request = PiePumps.Pumps[pumpId].PreviousRequest;    // give back the previous state

				OnCancelRequestStatus(this, new CancelRequestStatusEventArgs(
											PiePumps.Pumps[pumpId].PumpId,
											PiePumps.Pumps[pumpId].ZapOrderId,
											false));

				if (DEBUG_MODE)
				{
					Debug.WriteLine("-> processCancel Failed / Timeout for Pump Number " + pumpId);
				}

			}
			else if (PiePumps.Pumps[pumpId].CurrentStatus == PumpStatus.Idle)
			{
				PiePumps.Pumps[pumpId].Request = RequestPumpStatus.IdleNoRequest;
				OnCancelRequestStatus(this, new CancelRequestStatusEventArgs(
											PiePumps.Pumps[pumpId].PumpId,
											PiePumps.Pumps[pumpId].ZapOrderId,
											true));

				if (DEBUG_MODE)
				{
					Debug.WriteLine("-> processCancel Success for Pump Number " + pumpId);
				}
			}
		}

		private void UiPumpUpdate(int pumpNumber)
		{
			string ClientReference = "", ClientActivity = "";

			Color fuelStatusColor = Color.White;
			string fuelStatus = GetPumpStatus(pumpNumber, ref fuelStatusColor);

			if (fuelStatus == "")
				return; //do not do anything

			OnPumpIconChanged(this, new PumpIconEventArgs(pumpNumber, fuelStatus, fuelStatusColor,
														  ClientReference,
														  ClientActivity));

		}

		public string GetPumpStatus(int thePump, ref Color pumpColor)
		{
			try
			{
				switch (PiePumps.Pumps[thePump].CurrentStatus)
				{
					case PumpStatus.Init:
						pumpColor = Color.DeepPink;
						return "Offline";
					case PumpStatus.Idle:
						pumpColor = Color.DarkGray;
						return "Idle";
					case PumpStatus.Reserved:
						pumpColor = Color.DarkOrange;
						return "Reserved";
					case PumpStatus.StartedDispend:
						pumpColor = Color.DarkOrange;
						return "Nozzle Up";
					case PumpStatus.Authorized:
						pumpColor = Color.ForestGreen;
						return "Ready";
					case PumpStatus.Dispensing:
						pumpColor = Color.RoyalBlue;
						return "Delivering";
					case PumpStatus.DispenseCompleted:
						pumpColor = Color.ForestGreen;
						return "Completed";
					case PumpStatus.NoRespond:
						pumpColor = Color.DarkRed;
						return "NoRespond";
					case PumpStatus.NoAuthorise:
						pumpColor = Color.MediumVioletRed;
						return "NoAuthorise";
					case PumpStatus.PumpBlocked:
						pumpColor = Color.PaleVioletRed;
						return "Pump Blocked";
					default:
						return "";
				}
			}
			catch (Exception)
			{
				pumpColor = Color.OrangeRed;
				return "Error";
			}
		}


		private async Task updatePumpStatus(string allPumpStatus)
		{
			/*
                     28 02 02 02 02 2F 02 12 02 02 02 02 02 0000000000000000000000000000000000000000
                     28 02 02 02 02 2F 02 12 02 02 02 02020000000000000000000000000000000000000000
                        2017/10/25 23:34:38 disp no=1 DSTAT_IDLE, DISP_IDLE
                        2017/10/25 23:34:38 disp no=2 DSTAT_IDLE, DISP_IDLE
                        2017/10/25 23:34:38 disp no=3 DSTAT_IDLE, DISP_IDLE
                        2017/10/25 23:34:38 disp no=4 DSTAT_IDLE, DISP_IDLE
                        2017/10/25 23:34:38 disp no=5 DSTAT_RUNNING, DISP_RUNNING
                        2017/10/25 23:34:38 disp no=6 DSTAT_IDLE, DISP_IDLE
                        2017/10/25 23:34:38 disp no=7 DSTAT_SALECOMPLETE, DISP_RUNNING
                        2017/10/25 23:34:38 disp no=8 DSTAT_IDLE, DISP_IDLE
                        2017/10/25 23:34:38 disp no=9 DSTAT_IDLE, DISP_IDLE
                        2017/10/25 23:34:38 disp no=10 DSTAT_IDLE, DISP_IDLE
                        2017/10/25 23:34:38 disp no=11 DSTAT_IDLE, DISP_IDLE
                        2017/10/25 23:34:38 disp no=12 DSTAT_IDLE, DISP_IDLE

                     */

			if (allPumpStatus == "")
				allPumpStatus = "0000000000000000000000000000000000000000000000000000000000000";

			PiePumps.pumpStatusUpdate(allPumpStatus);

			if (previousMessageStatus != allPumpStatus)
			{
				for (int i = 0; i < PiePumps.TotalOfPump; i++)
				{
					UiPumpUpdate(i);
				}

				bool NotRequireToSendReserveState = true;

				if (allPumpStatus.Contains("#").Equals(false) && NotRequireToSendReserveState)
				{
					OnPumpStatus(this, new PumpStatusEventArgs(
						SystemParameters.RetailerNumber,
						PiePumps.TotalOfPump,
						allPumpStatus));
				}

				previousMessageStatus = allPumpStatus;
    
				SystemLogs.LogBankServer(allPumpStatus, "PI", "Pump Status");
			}
		}

		public async Task Write(byte[] dataToWrite)
		{
			try
			{
				//string temp = Encoding.UTF8.GetString(dataToWrite, 0, dataToWrite.Length);
				//Debug.WriteLine(">" + dataToWrite.Length + temp + "<");

				if (dataToWrite.Length < 5)
					return;

				//TcpClnt.WriteStream.Write(dataToWrite, 0, dataToWrite.Length);
				//await TcpClnt.WriteStream.FlushAsync();

				OnSendToPumpController(this, new SendToPumpControllerEventArgs(dataToWrite));
				await Task.Delay(500);

			}
			catch (Exception ex)
			{
				string dataBuffer = Encoding.UTF8.GetString(dataToWrite, 0, dataToWrite.Length);
				SystemLogs.LogError("WriteToSocket Exception", "Error Message: " + ex.Message,
					"Buffer:" + dataBuffer);
			}
		}
	}
	#endregion
}
