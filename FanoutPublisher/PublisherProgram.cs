using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace FanoutPublisher
{
    class PublisherProgram
    {
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
            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();

            channel.ExchangeDeclare("ex.fanout", "fanout", true, false, null);

            channel.QueueDeclare("myqueue1", true, false, false, null);
            channel.QueueDeclare("myqueue2", true, false, false, null);

            channel.QueueBind("myqueue1", "ex.fanout", "");
            channel.QueueBind("myqueue2", "ex.fanout", "");

            channel.BasicPublish("ex.fanout","",null,Encoding.UTF8.GetBytes("Message 1"));
            channel.BasicPublish("ex.fanout", "", null, Encoding.UTF8.GetBytes("Message 2"));

            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();

            channel.QueueDelete("my.queue1");
            channel.QueueDelete("my.queue2");
            channel.ExchangeDelete("ex.fanout");

            channel.Close();
            conn.Close();

        }
    }
}
