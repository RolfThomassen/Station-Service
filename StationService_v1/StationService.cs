using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;

namespace StationService_v1
{
    public partial class StationService : ServiceBase
    {
        private StationQueue _stationQueue;

        private string _userName = "user";
        private string _password = "ZcOX9DSmAdEq";
        private string _virtualHost = "/";
        private string _hostName = "104.215.184.134";
        private int _port = 5672;

        private Thread _pollStatusThread;
        private bool _pollFlag;

        public StationService()
        {
            InitializeComponent();

            _stationQueue = new StationQueue("RYW1166");
            _stationQueue.InitializeConn(_userName, _password, _hostName, _virtualHost, _port);

            if(_stationQueue.Connected)
            {
                _stationQueue.RegisterQueue(QueueConsumer);
            }

            _pollStatusThread = null;
            _pollFlag = false;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                // Update the service state to Start Pending.  
                ServiceStatus serviceStatus = new ServiceStatus();
                serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
                serviceStatus.dwWaitHint = 100000;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);

                if (_stationQueue.Initialized)
                {
                    _pollFlag = true;
                    _pollStatusThread = new System.Threading.Thread(PollMethod);
                    _pollStatusThread.Start();
                }

                // Update the service state to Running.  
                serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            }
            catch (Exception ex)
            {

            }
        }

        private void PollMethod()
        {
            while(_pollFlag)
            {
                _stationQueue.Publish("FE");
                Thread.Sleep(1000);
            }
            
        }

        protected override void OnStop()
        {
            try
            {
                ServiceStatus serviceStatus = new ServiceStatus();
                serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
                serviceStatus.dwWaitHint = 100000;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);

                _pollFlag = false;
                _pollStatusThread.Abort();

                // Update the service state to Running.  
                serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            }
            catch (Exception ex)
            {

            }
        }

        private void QueueConsumer(byte[] message)
        {
            //TODO: What to do with all the data
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
    }
}
