using System;
using Storiveo.IsisPie;

namespace StationServices
{
	public class Bootstrap
	{

		isisPiManager _isisPiManager = null;
		public Bootstrap()
		{
			_isisPiManager = new isisPiManager();
		}

		public void Start()
		{
			var info = "1: Pump Reserve. 2: Pump Authorise. 3: Pump Cancel";
			Console.WriteLine(info);

			while (true) // Loop indefinitely
			{
				info = "Enter Request Id :";
				Console.WriteLine(info);

				string input = Console.ReadLine(); // Get string from user

				int result = -1;
				if (int.TryParse(input, out result))
				{

					switch (result)
					{
						case 1:
							string zapOrderId = "stationServiceTest01";
							int pumpId = 1;

							_isisPiManager.RequestPumpReserve(zapOrderId, pumpId);
							break;

						case 2:
							zapOrderId = "stationServiceTest01";
							string transactionId = "stationServiceTest01";
							decimal authoriseLimit = 100;
							string webhook_start = "stationServiceTest01";
							string webhook_success = "stationServiceTest01";
							string webhook_error = "stationServiceTest01";

							_isisPiManager.RequestPumpAuthorise(zapOrderId, transactionId, authoriseLimit, webhook_start, webhook_success, webhook_error);
							break;

						case 3:
							zapOrderId = "stationServiceTest01";
							pumpId = 1;
							_isisPiManager.RequestPumpCancel(zapOrderId, pumpId);
							break;

						default:
							break;
					}

					Console.WriteLine(" Id entered:" + input);

				}
				else
				{
					Console.Write("Invalid ...");
				}
			}

		}

	}
}
