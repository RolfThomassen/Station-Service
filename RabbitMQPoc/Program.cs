using System;
using System.Threading;
using RabbitMQ.Client;

namespace RabbitMQPoc
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Rabbit MQ POC!");

            /*ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = "guest";
            factory.Password = "guest";
            factory.VirtualHost = "/";
            factory.HostName = "localhost";
            factory.Port = 5672;
            */
            try
            {
                Console.WriteLine("Creating Station MQ : RYB1065");
                var sq = new StationQueue("RYB1065");

                if(sq.Initialize())
                {
                    Console.WriteLine("Initialized Station MQ : RYB1065");

                    var i = 0;
                    while (i < 10)
                    {
                        byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes("M0" + i.ToString());
                        sq.Publish(messageBodyBytes);

                        Thread.Sleep(1000);

                        i++;
                    }
                }

                sq.Close();
                return;
            }
            catch(Exception ex)
            {
                
            }
        }
    }
}
