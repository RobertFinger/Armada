﻿using Models.Models;
using RabbitMQ.Client;
using System.Text;

namespace AccountsManager.Services
{
    public class AccountsManagerSender
    {
        private readonly IAsyncConnectionFactory _factory;

        public AccountsManagerSender(ILogger<AccountsManagerSender> logger, IAsyncConnectionFactory factory, IConfiguration configuration)
        {
            _logger = logger;
            _factory = factory;
            _configuration = configuration;

            //first run a rabbit mq server. https://www.rabbitmq.com/download.html
            // docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.12-management
            //for now use guest / guest for user and pass.
            //click on the docker container to test.

            // don't forget to set the startup projects to multiple projects and select both the sender and receivers.

        }

        public bool SendMessage(MessageDestination destination, string message)
        {
            try
            {
                StartMessageService(destination, Encoding.UTF8.GetBytes(message));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error starting message service {ex}", ex);
                return false;
            }
        }

        private ILogger<AccountsManagerSender> _logger { get; }
        private IConfiguration _configuration { get; }

        private void StartMessageService(MessageDestination destination, byte[] message)
        {         

            _factory.Uri = new Uri(_configuration[AuthConstants.RabbitMqUri]);
            _factory.ClientProvidedName = "Accounts Sender";

            IConnection connection = _factory.CreateConnection();
            IModel channel = connection.CreateModel();
            channel.BasicQos(0, 1, false);

            string exchangeName = "exchangerz";
            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: false, autoDelete: false, arguments: null);

            channel.QueueDeclare(queue: $"{destination}  Queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queue: $"{destination}  Queue", exchange: exchangeName, routingKey: destination.ToString(), arguments: null);
            
            channel.BasicPublish(exchange: exchangeName, routingKey: destination.ToString(), basicProperties: null, body: message);

            channel.Close();
            connection.Close();
        }
    }
}
