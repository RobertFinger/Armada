using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Models.Services; 
using Models.Models;

namespace YourNamespace.Services
{
    public class DataManagerReceiver : IHostedService
    {
        private readonly ILogger<DataManagerReceiver> _logger;
        private readonly IAsyncConnectionFactory _factory;
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection _connection;
        private IModel _channel;
        private readonly string _exchangeName = "exchangerz";
        private readonly string _queueName = "Data Queue";
        private readonly string _routingKey = "Data";

        public DataManagerReceiver(ILogger<DataManagerReceiver> logger, IAsyncConnectionFactory factory, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _factory = factory;
            _scopeFactory = scopeFactory;

            _factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            _factory.ClientProvidedName = "Data Management Receiver";

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct, durable: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(_queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(_queueName, _exchangeName, _routingKey, arguments: null);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            StartListening();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            StopListening();
            return Task.CompletedTask;
        }

        private void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                await SaveUserAsync(message);
            };

            _channel.BasicConsume(_queueName, autoAck: true, consumer: consumer);
        }

        private async Task SaveUserAsync(string message)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (message.TryParseJson(out User result))
                {
                    try
                    {
                        dbContext.Users.Add(result);
                        await dbContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical("Oops, {ex}", ex);
                    }
                }
                else
                {
                    _logger.LogError("Didn't parse the user.");
                }
            }
        }

        private void StopListening()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
