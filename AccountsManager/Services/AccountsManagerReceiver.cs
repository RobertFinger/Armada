using Models.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace AccountsManager.Services
{

    public class AccountsManagerReceiver 
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchangeName = "exchangerz";
        private readonly string _routingKey = "Accounts";
        private readonly string _queueName = "Accounts Queue";
        private readonly ILogger<AccountsManagerReceiver> _logger;
        private readonly IAsyncConnectionFactory _factory;
        private readonly AccountsManagerSender _sender;
        private readonly IConfiguration _configuration;

        public AccountsManagerReceiver(IConfiguration configuration, ILogger<AccountsManagerReceiver> logger, IAsyncConnectionFactory factory, AccountsManagerSender sender)
        {

            _logger = logger;
            _factory = factory;
            _sender = sender;
            _configuration = configuration;
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _factory.Uri = new Uri(_configuration[AuthConstants.RabbitMqUri]);
            _factory.ClientProvidedName = "Account Management Receiver";

            _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct, durable: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: _routingKey, arguments: null);

        }

        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received message: {message}");

                _logger.LogInformation("{Message}", message);
                _sender.SendMessage(Models.Models.MessageDestination.Gateway, "Accounts replying to gateway.");
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        }

        public void StopListening()
        {
            _channel.Close();
            _connection.Close();
        }
    }

}
