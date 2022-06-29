using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Publisher
{
    class PublisherProgram
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
           
            while (true)
            {
                Console.WriteLine("Enter message.");
                string message = Console.ReadLine();
                if (message == "exit")
                {
                    break;
                }
                else
                {
                    channel.BasicPublish("ex.fanout", "", null, Encoding.UTF8.GetBytes(message));

                }
            }
            channel.Close();
            conn.Close();
        }
    }
}
