using Faker;
using RabbitMQ.Client;
using System;
using System.Text;

namespace ConsoleLineApp
{
    public class Program
    {
        private static ConnectionFactory _factory;

        private static IConnection _connection;
        private static IModel _channel;

        private static string _username = "m";
        private static string _password = "123123";
        private static readonly string _queueName = "RealtimeQueue";

        private static readonly string _queueName1 = "RealtimeQueue1";
        private static readonly string _exchangeName = "RealtimeExchange";

        private static readonly string _routingKey = "routingkey";
        private static readonly string _routingKey1 = "routingkey1";
        private static readonly string _virtualHost = "Realtime";

        private static void Main(string[] args)
        {
            DelareQueue();
            BindQueue();

            // Loopgin to random string and enqueue first
            for (var i = 0; i < 101; i++)
            {
                var randomString = "[" + i + "] : " + Lorem.Sentence(10);
                Console.WriteLine("Number : [{0}] with :" + randomString, i);

                var obj = Encoding.ASCII.GetBytes(randomString);

                EnQueue(obj);
            }

            // Loopgin to random string and enqueue seconde
            for (var i = 0; i < 100; i++)
            {
                var randomString = "[" + i + "] : " + Lorem.Sentence(10);
                Console.WriteLine("Number : [{0}] with :" + randomString, i);

                var obj = Encoding.ASCII.GetBytes(randomString);

                EnQueue1(obj);
            }
        }

        public static void DelareQueue()
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

            _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: _queueName1, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        private static void BindQueue()
        {
            _channel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: _routingKey, arguments: null);
            _channel.QueueBind(queue: _queueName1, exchange: _exchangeName, routingKey: _routingKey1, arguments: null);
        }

        public static void EnQueue(byte[] line)
        {
            _channel.BasicPublish(exchange: _exchangeName, routingKey: _routingKey, mandatory: false, basicProperties: null, body: line);
        }

        private static void EnQueue1(byte[] line)
        {
            _channel.BasicPublish(exchange: _exchangeName, routingKey: _routingKey1, mandatory: false,
                basicProperties: null, body: line);
        }
    }
}