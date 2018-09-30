using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Storiveo.IsisPie;

namespace Storiveo.IsisPie
{
    public class StorageAccess
    {
        private readonly MessagesBase _msg;
        public string ProductAuthorize;
        public int DispenserId;
        public int DeviceId;
        public double DApprovedAmount;
        public string CardNo;

//        private readonly ISalesInfoContext _localStorageService;

        public StorageAccess()
        {
  //          _localStorageService = SalesInfoContext.Instance;
            // _localStorageService = new LocalStorageService();
        }

        public StorageAccess(ref MessagesBase messagesBase)
        {
            _msg = messagesBase;
    //        _localStorageService = SalesInfoContext.Instance;
            //_localStorageService = new LocalStorageService();
        }

        /*
        public void AddBankServerLog(string source, string message, string detailMessage = "")
        {
            var request = new BankServerLog()
            {
                DateTime = DateTime.Now.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToString("T"),
                Source = source,
                Message = detailMessage,
                MessageDetail = message
            };
            //_localStorageService.SaveBankServerLog(new BankServerLogViewModel(request));
            _localStorageService.SaveRecord(request);
        }

        public void getCurrentPumpRequest(string source, string message, string detailMessage = "")
        {
            var request = new BankServerLog()
            {
                DateTime = DateTime.Now.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToString("T"),
                Source = source,
                Message = detailMessage,
                MessageDetail = message
            };
            //_localStorageService.SaveBankServerLog(new BankServerLogViewModel(request));
            _localStorageService.SaveRecord(request);
        }

        public async Task<IList<ZapTransactions>> GetPumpRequestStatus()
        {
            return await _localStorageService.GetPumpRequestStatus();
        }
        */
    }
}
