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
            IModel chanel = conn.CreateModel();

            chanel.ExchangeDeclare("ex.fanout", "fanout", true, false, null);

            chanel.QueueDeclare("myqueue1", true, false, false, null);
            chanel.QueueDeclare("myqueue2", true, false, false, null);

            chanel.QueueBind("myqueue1", "ex.fanout", "");
            chanel.QueueBind("myqueue2", "ex.fanout", "");

            chanel.BasicPublish("ex.fanout","",null,Encoding.UTF8.GetBytes("Message 1"));
            chanel.BasicPublish("ex.fanout", "", null, Encoding.UTF8.GetBytes("Message 2"));

            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();

            chanel.QueueDelete("my.queue1");
            chanel.QueueDelete("my.queue2");
            chanel.ExchangeDelete("ex.fanout");

            chanel.Close();
            conn.Close();

        }
    }
}
