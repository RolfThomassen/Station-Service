using System;
using System.IO;
using Storiveo.IsisPie;
using Storiveo.IsisPie.EventArgs;
using Storiveo.IsisPie.MessageTypes;

namespace Storiveo.IsisPie
{
    public static class SystemLogs
    {
        public static bool DebugMode = false;
        public static bool LogErrorQuery = true;
        public static bool LogBankServerComm = true;

        public static bool saveToDb = true;

        private static readonly StorageAccess CommLog = new StorageAccess();


        public static void LogError(string msg, string stkTrace, string title)
        {
            if (!LogErrorQuery)
                return;

            title = title.Replace("\0", "");
            //CommLog.AddBankServerLog(title, msg, stkTrace);
        }
        
        public static void LogBankServer(string message, string dataFrom, string detail)
        {
            if (!LogBankServerComm)
                return;

            message = message.Replace("\0", "");
            //CommLog.AddBankServerLog(dataFrom, message, detail);
        }
    }
}
