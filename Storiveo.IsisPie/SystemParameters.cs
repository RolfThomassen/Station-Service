using System.Threading.Tasks;

namespace Storiveo.IsisPie
{
    public static class SystemParameters
    {
        public static int reserveTimout = 60;
        public static int authoriseTimout = 170;
        public static int cancelTimout = 10;
        public static string BankServerIp = "";
        public static string BankServerPort = "";
		public static string RetailerNumber = "";

        public static async Task GetSettingAsync ()
        {
            
            //SettingManager settings = new SettingManager();

            //var setting = await settings.GetAllSettings();

			BankServerIp = ""; //setting.BankServerIp;
			BankServerPort = ""; //setting.BankServerPort;
			RetailerNumber = ""; //setting.RetailerId;
        }
    }
}