using Middleware;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQSender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQReceiver
{
    class Program
    {
        private static MyMessage deserialized;

        static void Main(string[] args)
        {
            ReceiveSerialisationMessages();

        }

        private static void ReceiveSerialisationMessages()
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                RabbitMqService rs = new RabbitMqService();
                rs.SetupInitialTopicQueue(channel);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    deserialized = JsonConvert.DeserializeObject<MyMessage>(message);
                    Thread.Sleep(1000);

                    Console.WriteLine("have a break" + deserialized.body);

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(RabbitMqService.SerialisationQueueName,
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }


    }
}
