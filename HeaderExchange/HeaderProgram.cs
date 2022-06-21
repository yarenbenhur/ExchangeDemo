using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace HeaderExchange
{
    class HeaderProgram
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

            channel.ExchangeDeclare("ex.header", "header", true, false, null);

            channel.QueueDeclare("my.queue1", true, false, false, null);
            channel.QueueDeclare("my.queue2", true, false, false, null);

            channel.QueueBind("my.queue1", "ex.header", "",new Dictionary<string, object>() 
            {
                {"x-match","all" },
                {"job","convert" },
                {"format","jpeg" }

            });
            channel.QueueBind("my.queue2", "ex.header", "", new Dictionary<string, object>()
            {
                {"x-match","any" },
                {"job","convert" },
                {"format","jpeg" }

            });
            IBasicProperties props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("job", "convert");
            props.Headers.Add("format", "jpeg");


            channel.BasicPublish("ex.header", "", props, Encoding.UTF8.GetBytes("Message 1"));

            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("job", "convert");
            props.Headers.Add("format", "bitmap");

            channel.BasicPublish("ex.header", "", props, Encoding.UTF8.GetBytes("Message 2"));

        }
    }
}
