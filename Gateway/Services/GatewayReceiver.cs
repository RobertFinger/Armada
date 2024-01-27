using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace Gateway.Services
{

    public class GatewayReceiver 
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchangeName = "exchangerz";
        private readonly string _routingKey = "route keyz";
        private readonly string _queueName = "queue namez";
        private readonly ILogger<GatewayReceiver> _logger;
        private readonly IAsyncConnectionFactory _factory;

        public GatewayReceiver(ILogger<GatewayReceiver> logger, IAsyncConnectionFactory factory)
        {

            _logger = logger;
            _factory = factory;

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            _factory.ClientProvidedName = "Gateway Receiver";

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
