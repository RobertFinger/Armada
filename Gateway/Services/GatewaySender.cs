using System.Text;
using RabbitMQ.Client;

namespace Gateway.Services
{
    public class GatewaySender
    {
        private readonly IAsyncConnectionFactory _factory;

        public GatewaySender(ILogger<GatewaySender> logger, IAsyncConnectionFactory factory)
        {
            Logger = logger;
            _factory = factory;
            StartMessageService();

            //first run a rabbit mq server. https://www.rabbitmq.com/download.html
            // docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.12-management
            //for now use guest / guest for user and pass.
            //click on the docker container to test.

            // don't forget to set the startup projects to multiple projects and select both the sender and receivers.

        }

        public ILogger<GatewaySender> Logger { get; }

        public void StartMessageService()
        {

            _factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            _factory.ClientProvidedName = "Gateway Sender";

            IConnection connection = _factory.CreateConnection();
            IModel channel = connection.CreateModel();

            string exchangeName = "exchangerz";

            string amRoutingKey = "Accounts";
            string dmRoutingKey = "Data";
            string gmRoutingKey = "Game";
            string lmRoutingKey = "Lobby";
            string pmRoutingKey = "Payments";


            string amQueueName = "Accounts Queue";
            string dmQueueName = "Data Queue";
            string gmQueueName = "Game Queue";
            string lmQueueName = "Lobby Queue";
            string pmQueueName = "Payments Queue";

            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: false, autoDelete: false, arguments: null);
            
            channel.QueueDeclare(queue: amQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: dmQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: gmQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: lmQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: pmQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            channel.QueueBind(queue: amQueueName, exchange: exchangeName, routingKey: amRoutingKey, arguments: null);
            channel.QueueBind(queue: dmQueueName, exchange: exchangeName, routingKey: dmRoutingKey, arguments: null);
            channel.QueueBind(queue: gmQueueName, exchange: exchangeName, routingKey: gmRoutingKey, arguments: null);
            channel.QueueBind(queue: lmQueueName, exchange: exchangeName, routingKey: lmRoutingKey, arguments: null);
            channel.QueueBind(queue: pmQueueName, exchange: exchangeName, routingKey: pmRoutingKey, arguments: null);

            var mesageForAccountsManager = Encoding.UTF8.GetBytes("Gateway to Accounts Manager, come in Accounts Manager!");
            var mesageForDataManager = Encoding.UTF8.GetBytes("Gateway to Data Manager, come in Data Manager!");
            var mesageForGameManager = Encoding.UTF8.GetBytes("Gateway to Game Manager, come in Game Manager!");
            var mesageForlobbyManager = Encoding.UTF8.GetBytes("Gateway to Lobby Manager, come in Lobby Manager!");
            var mesageForPaymentManager = Encoding.UTF8.GetBytes("Gateway to Payment Manager, come in Payment Manager!");

            channel.BasicPublish(exchange: exchangeName, routingKey: amRoutingKey, basicProperties: null, body: mesageForAccountsManager);
            channel.BasicPublish(exchange: exchangeName, routingKey: dmRoutingKey, basicProperties: null, body: mesageForDataManager);
            channel.BasicPublish(exchange: exchangeName, routingKey: gmRoutingKey, basicProperties: null, body: mesageForGameManager);
            channel.BasicPublish(exchange: exchangeName, routingKey: lmRoutingKey, basicProperties: null, body: mesageForlobbyManager);
            channel.BasicPublish(exchange: exchangeName, routingKey: pmRoutingKey, basicProperties: null, body: mesageForPaymentManager);

            channel.Close();
            connection.Close();
        }
    }
}
