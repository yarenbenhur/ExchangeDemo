using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WorkQueues
{
    class WorkProgram
    {
        static IConnection conn;
        static IModel channel;
        static void Main(string[] args)
        {
            //used to distribute tasks among multiple workers.
            //Producers add tasks to a queue and these tasks are distributed to multiple worker applications.
            //Pull or push models can be used to distribute tasks among the workers.

            Console.WriteLine("Enter workers name.");
            string Name = Console.ReadLine();
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

            channel.BasicQos(0, 1, false); // do not send new message if the worker did not acknowlaged the last message yet.
                                           // Need to send ack manually like FanoutConsumer.prj
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body);
                Console.WriteLine($"[{Name}] Message:{message}");
                
                int durationInSeconds = Int32.Parse(message);
                Thread.Sleep(durationInSeconds * 1000);
                Console.WriteLine("FINISHED");

                channel.BasicAck(e.DeliveryTag, false); // manually ack 

            };

            string consumerTag = channel.BasicConsume("my.queue1", false, consumer); // autoact = false
            Console.WriteLine("Subscribed. Press a key to unsubscribe and exit.");
            Console.ReadKey();

            channel.BasicCancel(consumerTag);
            channel.Close();
            conn.Close();
        }
    }
}
