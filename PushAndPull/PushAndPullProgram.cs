using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PushAndPull
{
    class PushAndPullProgram
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

            //readMessagesWithPushModel();
            readMessagesWithPullModel();

            channel.Close();
            conn.Close();
        }
        private static void readMessagesWithPushModel()
        {
            //Consumer application subscribes to the queue and waits for messages. 
            //If there is already a message on the queue.
            //Or when a new message arrives, it is automatically sent (pushed)to the consumer application.

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body);
                Console.WriteLine("Message:" + message);
            };

            string consumerTag = channel.BasicConsume("my.queue1", true, consumer);

            Console.WriteLine("Subscribed. Press a key to unsubscribe and exit.");
            Console.ReadKey();

            channel.BasicCancel(consumerTag);
        }
        private static void readMessagesWithPullModel()
        {
            //Consumer application does not subscribe to the queue.
            //But it constantly checks(polls) the queue for new messages.
            //If there is a message available on the queue, it is manually fetched (pulled)by the consumer application.

            Console.WriteLine("Reading messages from queue. Press 'e' to exit.");

            while (true) //check queue for messages
            {
                Console.WriteLine("Trying to get message from the queue...");
                BasicGetResult result = channel.BasicGet("my.queue1", true);
                if (result != null)
                {
                    string message = Encoding.UTF8.GetString(result.Body);
                    Console.WriteLine("Message:" + message);
                }
                if (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey();
                    if (keyInfo.KeyChar == 'e' || keyInfo.KeyChar == 'E')
                    {
                        return;
                    }
                }
                Thread.Sleep(2000);
            }
        }
    }
}