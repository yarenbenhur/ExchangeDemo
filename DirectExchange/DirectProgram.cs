using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace DirectExchange
{
    class DirectProgram
    {
        static IConnection conn;
        static IModel channel;
        static void Main(string[] args)
        {
            //Routes messages to the queues based on the "routing key" specified in binding definition.

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

            channel.ExchangeDeclare("ex.direct", "direct", true, false, null);

            channel.QueueDeclare("my.info", true, false, false, null);
            channel.QueueDeclare("my.warning", true, false, false, null);


            channel.QueueBind("my.info", "ex.direct", "info"); // routing key = info
            channel.QueueBind("my.warning", "ex.direct", "warning"); // routing key = warning

            channel.BasicPublish("ex.direct", "info", null, Encoding.UTF8.GetBytes("Message with routing key info")); //routing key = info
            channel.BasicPublish("ex.direct", "warning", null, Encoding.UTF8.GetBytes("Message with routing key warning")); //routing key = warning

            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();

            channel.QueueDelete("my.info");
            channel.QueueDelete("my.warning");
            channel.ExchangeDelete("ex.direct");

            channel.Close();
            conn.Close();

        }
    }
}
