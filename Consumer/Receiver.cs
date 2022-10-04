﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
public class Receiver
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };


        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "helloQueue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            Console.WriteLine($"Waiting for recieving messages in host: {factory.HostName}");


            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);

                int dots = message.Split('.').Length - 1;
                Thread.Sleep(dots * 1000);

                Console.WriteLine(" [x] Done");

                 // Note: it is possible to access the channel via
                //       ((EventingBasicConsumer)sender).Model here
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                
            };

            channel.BasicConsume(queue: "helloQueue",
                                 autoAck: false,
                                 consumer: consumer);


            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}