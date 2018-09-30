using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace StationService_v1
{
    public class RabbitMessaging : IMessaging
    {
        private IConnection _connection;
        private IModel _channel;
        private EventingBasicConsumer _consumer;
        private string _consumerTag;
        private string _stationName;
        private string _consumeQueueName;
        private string _publishQueueName;
        private bool _connected;
        private bool _initialized;

        delegate void Del(string message);

        public RabbitMessaging(string stationName, string publishQueueName, string consumeQueueName)
        {
            _stationName = stationName;
            _consumeQueueName = consumeQueueName;
            _publishQueueName = publishQueueName;
            _connected = false;
            _initialized = false;
        }

        public bool Connected { get { return _connected; } }

        public bool Initialized { get { return _initialized; } }

        public bool InitializeConnection(string userName, string password, string hostName, string vhost, int port)
        {
            if (_connection != null)
            {
                ConnectionFactory factory = new ConnectionFactory();
                factory.UserName = userName;
                factory.Password = password;
                factory.VirtualHost = vhost;
                factory.HostName = hostName;
                factory.Port = port;

                try
                {
                    _connection = factory.CreateConnection();
                    _connected = true;
                    return true;
                }
                catch(Exception ex)
                {

                }
            }
            return false;
        }

        public bool InitializeQueues(ConsumerDel consumerFunction)
        {
            if (_connection != null)
            {
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(_stationName, ExchangeType.Direct);

                _channel.QueueDeclare(_publishQueueName, false, false, false);
                _channel.QueueDeclare(_consumeQueueName, false, false, false);

                _channel.QueueBind(_publishQueueName, _stationName, _publishQueueName);
                _channel.QueueBind(_consumeQueueName, _stationName, _consumeQueueName);

                _consumer = new EventingBasicConsumer(_channel);

                _consumer.Received += (ch, ea) =>
                {
                    var body = ea.Body;
                    _channel.BasicAck(ea.DeliveryTag, false);

                    consumerFunction(body);
                };
                _consumerTag = _channel.BasicConsume(_consumeQueueName, false, _consumer);

                _initialized = true;
            }

            return _initialized;
        }

        public void PublishToQueue(byte[] data)
        {
            _channel.BasicPublish(_stationName, _publishQueueName, null, data);
        }

        public void PublishToQueue(string data)
        {
            _channel.BasicPublish(_stationName, _publishQueueName, null, System.Text.Encoding.UTF8.GetBytes(data));
        }
    }
}
