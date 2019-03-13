using RabbitMQ.Client;

namespace Middleware
{
    public class RabbitMqService
    { 
        public static string SerialisationQueueName = "queueForObjectToBeSent";
        public static string SerialisationExchangeName = "exchangeForObjectToBeSent";
        public static string SerialisationRoutingKey = "routingKey";

        private static volatile IConnection _connection;
        private static readonly object ConnectionLock = new object();
        private static volatile IModel _channel;
        private static readonly object ChannelLock = new object();
        
        public IConnection RabbitMqConnection
        {
            get
            {
                if (_connection != null)
                {
                    return _connection;
                }
                lock (ConnectionLock)
                {
                    if (_connection != null)
                    {
                        return _connection;
                    }
                    var connectionFactory = new ConnectionFactory
                    {
                        HostName = "localhost",
                        UserName = "guest",
                        Password = "guest",
                        AutomaticRecoveryEnabled = true
                    };
                    _connection = connectionFactory.CreateConnection();
                }
                return _connection;
            }
        }

        public IModel RabbitMqModel
        {
            get
            {
                if (_channel != null)
                {
                    return _channel;
                }
                lock (ChannelLock)
                {
                    if (_channel != null)
                    {
                        return _channel;
                    }
                    _channel = RabbitMqConnection.CreateModel();
                }
                return _channel;
            }
        }
        public void SetupInitialTopicQueue(IModel model)
        {
            model.QueueDeclare(SerialisationQueueName, durable: true, exclusive: false, autoDelete: false, null);
            model.ExchangeDeclare(SerialisationExchangeName, ExchangeType.Direct);
            model.QueueBind(SerialisationQueueName, SerialisationExchangeName, SerialisationRoutingKey);
        }

    }
}
