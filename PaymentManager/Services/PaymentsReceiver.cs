﻿using Models.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace PaymentManager.Services
{

    public class PaymentsReceiver 
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchangeName = "exchangerz";
        private readonly string _routingKey = "Payments";
        private readonly string _queueName = "Payments Queue";
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentsReceiver> _logger;
        private readonly IAsyncConnectionFactory _factory;
        private readonly PaymentSender _sender;

        public PaymentsReceiver(IConfiguration configuration, ILogger<PaymentsReceiver> logger, IAsyncConnectionFactory factory, PaymentSender sender)
        {

            _logger = logger;
            _factory = factory;
            _sender = sender;
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _configuration = configuration;

            _factory.Uri = new Uri(_configuration[AuthConstants.RabbitMqUri]);
            _factory.ClientProvidedName = "Payments Receiver";

            _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct, durable: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: _routingKey, arguments: null);

        }

        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                Thread.Sleep(1000);
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received message: {message}");
                _logger.LogInformation("{Message}", message);
                _sender.SendMessage(Models.Models.MessageDestination.Gateway, "Got your message, gateway, and payments wilco");
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
