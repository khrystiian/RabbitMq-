using Middleware;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Impl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQSender
{
    class Program
    {
        static void Main(string[] args)
        {
            RunSerialisationDemo();
        }

        private static void RunSerialisationDemo()
        {
            RabbitMqService commonService = new RabbitMqService();

                using (var connection = commonService.RabbitMqConnection)
                {
                    using (var channel = commonService.RabbitMqModel)
                    {
                    while (true)
                    {
                        commonService.SetupInitialTopicQueue(channel);
                        var random = new Random();
                        MyMessage customer = new MyMessage()
                        {
                            body = "this is the body of the message",
                            someVal = random.Next(1, 20)
                        };
                        var basicProperties = channel.CreateBasicProperties();
                        basicProperties.DeliveryMode = 2;
                        string jsonified = JsonConvert.SerializeObject(customer);
                        byte[] customerBuffer = Encoding.UTF8.GetBytes(jsonified);
                        //container for the exchange information
                        //  PublicationAddress address = new PublicationAddress(ExchangeType.Direct, RabbitMqService.SerialisationExchangeName, RabbitMqService.SerialisationQueueExchangeBinding);
                        channel.BasicPublish(RabbitMqService.SerialisationExchangeName, RabbitMqService.SerialisationRoutingKey, basicProperties, customerBuffer);
                        Console.WriteLine("Published message: " + jsonified);
                        Thread.Sleep(1000);
                    }
                }

            }

        }
    }
}
