using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationService_v1
{
    public class StationQueue
    {
        private string _stationName;
        private const string _readQueue = "queue_2";
        private const string _writeQueue = "queue_1";
        private IMessaging _iMessaging;

        public StationQueue(string stationName)
        {
            _stationName = stationName;
        }

        public bool InitializeConn(string userName, string password, string hostName, string vHost, int port)
        {
            _iMessaging = new RabbitMessaging(_stationName, _writeQueue, _readQueue);
            return _iMessaging.InitializeConnection(userName, password, hostName, vHost, port);
        }

        public bool Connected { get { return _iMessaging.Connected; } }

        public bool Initialized { get { return _iMessaging.Initialized; } }

        public bool RegisterQueue(ConsumerDel callBackFunc)
        {
            if (_iMessaging != null)
            {
                if (_iMessaging.Connected)
                {
                    return _iMessaging.InitializeQueues(callBackFunc);
                }
            }

            return false;
        }

        public void Publish(byte[] message)
        {
            _iMessaging.PublishToQueue(message);
        }

        public void Publish(string message)
        {
            _iMessaging.PublishToQueue(message);
        }
    }
}
