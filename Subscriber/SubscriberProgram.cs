using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Subscriber
{
    class SubscriberProgram
    {
        static IConnection conn;
        static IModel channel;
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = "localhost",
                VirtualHost = "/",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            Console.WriteLine("Enter queues name.");
            string queueName = Console.ReadLine();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body);
                Console.WriteLine($"[{queueName}] Message:{message}");

            };
            string consumerTag = channel.BasicConsume(queueName, true, consumer);
            Console.WriteLine($"Subscribed to {queueName}. Press a key to unsubscribe and exit.");
            Console.ReadKey();

            //channel.BasicCancel(consumerTag);
            channel.Close();
            conn.Close();
        }
    }
}
