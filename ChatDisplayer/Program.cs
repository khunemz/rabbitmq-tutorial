using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace ChatDisplayer
{
    internal class Program
    {
        private static ConnectionFactory _factory;

        private static IConnection _connection;
        private static IModel _channel;

        private static string _username = "m";
        private static string _password = "123123";
        private static readonly string _queueName = "RealtimeQueue";
        private static readonly string _queueName1 = "RealtimeQueue1";
        private static readonly string _virtualHost = "Realtime";

        private static void Main(string[] args)
        {
            DeQueue();
        }

        public static void DeQueue()
        {
            _factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = _username,
                Password = _password,
                VirtualHost = _virtualHost,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine(message + " : " + ea.DeliveryTag.ToString());
                Thread.Sleep(50);

                // Done stamping to RabbitMQ server
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: _queueName, noAck: false, consumer: consumer);
            _channel.BasicConsume(queue: _queueName1, noAck: false, consumer: consumer);
        }
    }
}