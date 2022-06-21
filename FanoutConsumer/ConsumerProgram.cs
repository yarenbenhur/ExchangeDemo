using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FanoutConsumer
{
     class ConsumerProgram
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

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;

          
            var consumerTag = channel.BasicConsume("myqueue1", false, consumer); //auto act false
            Console.WriteLine("Waiting for messages. Press any key to exit.");
            Console.ReadKey();
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Body);
            Console.WriteLine("Message:"+ message);

            //channel.BasicAck(e.DeliveryTag, false); //giving act for received massage 
            channel.BasicNack(e.DeliveryTag, false,true); //rejection senerio, requeue=true  

            

        }
    }
}

