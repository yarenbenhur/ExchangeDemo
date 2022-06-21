using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace TopicExchange
{
    class TopicProgram
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

            channel.ExchangeDeclare("ex.topic", "topic", true, false, null);

            channel.QueueDeclare("my.queue1", true, false, false, null);
            channel.QueueDeclare("my.queue2", true, false, false, null);
            channel.QueueDeclare("my.queue3", true, false, false, null);

            channel.QueueBind("my.queue1", "ex.topic", "*.image.*"); // routing key = info
            channel.QueueBind("my.queue2", "ex.topic", "#.image"); // routing key = warning
            channel.QueueBind("my.queue3", "ex.topic", "image.#"); // routing key = warning

            channel.BasicPublish("ex.topic", "convert.bitmap.image", null, Encoding.UTF8.GetBytes("Routing key is convert.bitmap.image")); 
            channel.BasicPublish("ex.topic", "image.bitmap.32bit", null, Encoding.UTF8.GetBytes("Routing key is image.bitmap.32bit")); 
            channel.BasicPublish("ex.topic", "convert.image.bmp", null, Encoding.UTF8.GetBytes("Routing key is convert.image.bmp")); 
            channel.BasicPublish("ex.topic", "convert.image.bitmap.32bit", null, Encoding.UTF8.GetBytes("Routing key is convert.image.bitmap.32bit")); //wont be sended to any queue 

            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();

            channel.QueueDelete("my.info");
            channel.QueueDelete("my.warning");
            channel.ExchangeDelete("ex.topic");

            channel.Close();
            conn.Close();
        }
    }
}
