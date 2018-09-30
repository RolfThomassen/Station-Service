using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQPoc
{
    public class StationQueue
    {
        private const string _readQueue = "queue_2";
        private const string _writeQueue = "queue_1";
        private string _stationName;
        private IConnection _connection;
        private IModel _channel;
        private EventingBasicConsumer _consumer;
        private string _consumerTag;

        public StationQueue(string stationName)
        {
            _stationName = stationName;
        }

        public bool Initialize(IConnection connection = null)
        {
            try
            {
                if (connection != null)
                {
                    _connection = connection;
                }                    
                else
                {
                    ConnectionFactory factory = new ConnectionFactory();
                    factory.UserName = "user";
                    factory.Password = "ZcOX9DSmAdEq";
                    factory.VirtualHost = "/";
                    factory.HostName = "104.215.184.134";
                    factory.Port = 5672;

                    _connection = factory.CreateConnection();
                }

                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(_stationName, ExchangeType.Direct);

                _channel.QueueDeclare(_writeQueue, false, false, false);
                _channel.QueueDeclare(_readQueue, false, false, false);

                _channel.QueueBind(_writeQueue, _stationName, _writeQueue);
                _channel.QueueBind(_readQueue, _stationName, _readQueue);

                _consumer = new EventingBasicConsumer(_channel);
                _consumer.Received += (ch, ea) =>
                    {
                        var body = ea.Body;

                        Console.WriteLine(_stationName + ": in:" + System.Text.Encoding.UTF8.GetString(body));
                        _channel.BasicAck(ea.DeliveryTag, false);
                    };
                _consumerTag = _channel.BasicConsume(_readQueue, false, _consumer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failure to connect to RabbitMQ server");
                Console.WriteLine("Error :  " + ex.Message);

                Thread.Sleep(3000); //enought time to read message on console
                return false;
            }

            return true;
        }

        public void Publish(byte[] messageBodyBytes)
        {
            Console.WriteLine(_stationName + ": out:" + System.Text.Encoding.UTF8.GetString(messageBodyBytes));
            _channel.BasicPublish(_stationName, _writeQueue, null, messageBodyBytes);
        }

        public void Close()
        {
            if (_channel != null)
                _channel.Close();
        }
    }
}
