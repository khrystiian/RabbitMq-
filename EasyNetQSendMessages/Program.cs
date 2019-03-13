using EasyNetQ;
using Middleware;
using System;
using System.Threading;

namespace EasyNetQSendMessages
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * The message will not stay in the queue.
             * Without a subscriber, the message will just dissapear.
             * Should be in a separate process.
             */
            //Subscribe();
            //Publish();


            /*             
             *The message will stay in the queue until a consumer will receive it.      
             * Should be done in a separate process.
             */
            // Send();
            // Receive();


            /*
             * Send a request message and wait for a response to come back.
             * if no response is back, a timeout error will be thrown.
             * Can also be done async - recommended
             */
            //Response();
            //Request();
             
        }
        #region Request and Response 
        private static void Request()
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            var rand = new Random();
            while (true)
            {
                var myMessage = new MyMessage
                {
                    body = "this is MyMessage body",
                    someVal = rand.Next(1, 20)
                };
                Console.WriteLine("Sending a request.");
                var response = bus.Request<MyMessage, MyResponse>(myMessage);
                Console.WriteLine(response.Message);
                Thread.Sleep(1000);
            }
        }

        private static void Response()
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            bus.Respond<MyMessage, MyResponse>(x =>
            {
                return new MyResponse
                {
                    Message = string.Format("Response: {0}, {1}", x.body, x.someVal)
                };
            });
        }
        #endregion

        #region Send and Receive
        static void Send()
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            var rand = new Random();
            while (true)
            {
                var myMessage = new MyMessage
                {
                    body = "this is MyMessage body",
                    someVal = rand.Next(1, 20)
                };
                var myOtherMessage = new MyOtherMessage
                {
                    body = "this is MyOtherMessage body",
                    MyOtherMessageId = rand.Next(1, 20)
                };
                //these messages will be put in a queue even if there aren't any receivers yet.
                //Queue created automatically.
                bus.Send("my.queue", myMessage);
                bus.Send("my.queue", myOtherMessage);
                Console.WriteLine("Sent two different messages.");
                Thread.Sleep(1000);
            }
        }
        static void Receive()
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            bus.Receive("my.queue", x => x.Add<MyMessage>(m => {
                Console.WriteLine("Received MyMessage: {0}, {1}", m.someVal, m.body);
            })
            .Add<MyOtherMessage>(m => {
                Console.WriteLine("Received MyOtherMessage: {0}, {1}", m.MyOtherMessageId, m.body);
            }));

        }
        #endregion

        #region Publish and Subscribe
        static void Subscribe()
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            bus.Subscribe<MyMessage>("subscription_id", x =>   //subscription_id: useful when having multiple subscribers
            {
                Console.WriteLine("Received message: {0}, {1}", x.body, x.someVal);
            });
        }

        static void Publish()
        {
            var rand = new Random();
            var bus = RabbitHutch.CreateBus("host=localhost"); //connect to easyNetQ
            while (true)
            {
                var message = new MyMessage
                {
                    body = "the body of MyMessage",
                    someVal = rand.Next(1, 20)
                };
                bus.Publish(message);
                Console.WriteLine("Published a message");
                Thread.Sleep(1000);
            }
        }
        #endregion
    }
}
