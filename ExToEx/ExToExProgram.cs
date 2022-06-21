using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace ExToEx
{
    class ExToExProgram
    {
        static void Main(string[] args)
        {
            IConnection conn;
            IModel channel;
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

            channel.ExchangeDeclare("ex.1", "direct", true, false, null);
            channel.ExchangeDeclare("ex.2", "direct", true, false, null);

            channel.QueueDeclare("my.queue1", true, false, false, null);
            channel.QueueDeclare("my.queue2", true, false, false, null);

            channel.QueueBind("my.queue1", "ex.1", "key1");
            channel.QueueBind("my.queue2", "ex.2", "key2");

            channel.ExchangeBind("ex.2", "ex.1", "key2");

            channel.BasicPublish("ex.1", "key1", null, Encoding.UTF8.GetBytes("Message with routing key 1")); 
            channel.BasicPublish("ex.1", "key2", null, Encoding.UTF8.GetBytes("Message with routing key 2"));

            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();

            channel.QueueDelete("my.queue1");
            channel.QueueDelete("my.queue2");
            channel.ExchangeDelete("ex.1");
            channel.ExchangeDelete("ex.2");

            channel.Close();
            conn.Close();
        }
    }
}
