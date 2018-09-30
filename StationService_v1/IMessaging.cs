using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationService_v1
{
    public delegate void ConsumerDel(byte[] message);

    public interface IMessaging
    {
        bool Connected { get; }
        bool Initialized { get; }

        bool InitializeConnection(string userName, string password, string hostName, string vHost, int port);
        bool InitializeQueues(ConsumerDel consumerFunction);
        void PublishToQueue(byte[] data);
        void PublishToQueue(string data);
    }
}
