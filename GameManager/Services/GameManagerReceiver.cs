using Models.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace GameManager.Services
{

    public class GameManagerReceiver 
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchangeName = "exchangerz";
        private readonly string _routingKey = "LobbyGameData";
        private readonly string _queueName = "LobbyGameData Queue";
        private readonly ILogger<GameManagerReceiver> _logger;
        private readonly IAsyncConnectionFactory _factory;
        private readonly GameManagerSender _sender;

        public GameManagerReceiver(ILogger<GameManagerReceiver> logger, IAsyncConnectionFactory factory, GameManagerSender sender)
        {

            _logger = logger;
            _factory = factory;
            _sender = sender;
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            _factory.ClientProvidedName = "LobbyGameData Manager Receiver";

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
                _sender.SendMessage(MessageDestination.Gateway, "LobbyGameData manager received your message");
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
