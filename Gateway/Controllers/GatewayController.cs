using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Models.Models;
using System.Web.Http;
using Gateway.Services;
using System.Text.Json;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;

namespace Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GatewayController : ControllerBase, IDisposable
{
    private readonly ILogger<GatewayController> _logger;
    private readonly IConfiguration _configuration;
    private IConnection _connection;
    private readonly GatewaySender _messages;
    private IModel _channel;
    private readonly string _exchangeName = "exchangerz"; 
    private readonly string _queueName = $"{MessageDestination.Gateway} Queue";
    private readonly string _routingKey = MessageDestination.Gateway.ToString();
    private List<WebSocket> _webSockets = new List<WebSocket>();
    private bool _disposed = false;

    public GatewayController(ILogger<GatewayController> logger, IConfiguration configuration, GatewaySender messages)
    {
        _logger = logger;
        _configuration = configuration;
        _messages = messages;

        SetupRabbitMq();
    }

    private void SetupRabbitMq()
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_configuration[AuthConstants.RabbitMqUri]),
            ClientProvidedName = "Gateway Receiver"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(_exchangeName, type: ExchangeType.Direct, durable: false, autoDelete: false, arguments: null);
        _channel.QueueDeclare(_queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueBind(_queueName, _exchangeName, routingKey: _routingKey, arguments: null);

        StartListening();
    }

    private void StartListening()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation($"Received message: {message}");

            foreach (var webSocket in _webSockets.Where(ws => ws.State == WebSocketState.Open))
            {
                await SendWebSocketMessage(webSocket, message);
            }
        };

        _channel.BasicConsume(_queueName, autoAck: true, consumer: consumer);
    }

    private async Task SendWebSocketMessage(WebSocket webSocket, string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        var segment = new ArraySegment<byte>(buffer);
        await webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
    }


    [HttpGet("/MessengerTest")]
    public void MessengerTest()
    {
        var messageForAccountsManager = "Gateway to Accounts Manager, come in Accounts Manager!";
        var messageForDataManager = "Gateway to Data Manager, come in Data Manager!";
        var messageForGameManager = "Gateway to LobbyGameData Manager, come in LobbyGameData Manager!";
        var messageForLobbyManager = "Gateway to Lobby Manager, come in Lobby Manager!";
        var messageForPaymentManager = "Gateway to Payment Manager, come in Payment Manager!";

        _messages.SendMessage(MessageDestination.Accounts, messageForAccountsManager);
        _messages.SendMessage(MessageDestination.Data, messageForDataManager);
        _messages.SendMessage(MessageDestination.Game, messageForGameManager);
        _messages.SendMessage(MessageDestination.Lobby, messageForLobbyManager);
        _messages.SendMessage(MessageDestination.Payments, messageForPaymentManager);
    }

    [HttpGet("/RequestLobby"), Authorize(Roles = "User")]
    public ActionResult<Guid> GetLobby(Guid? lobby, int gameType, string callBackUri)
    {
        var requestId = lobby ?? Guid.NewGuid();

        var lobbyRequest = new LobbyRequest { RequestId = requestId, GameType = (GameType)gameType, CallbackUri = callBackUri };
        var requestJson = JsonSerializer.Serialize(lobbyRequest);

        _messages.SendMessage(MessageDestination.Lobby, requestJson);
        _logger.LogInformation("Lobby request sent");

        return Ok(lobbyRequest.RequestId);
    }

    [HttpPost("/SaveUser")]
    public ActionResult SaveUser([System.Web.Http.FromBody] User user)
    {
        var userJson = JsonSerializer.Serialize(user);
        _messages.SendMessage(MessageDestination.Data, userJson);
        _logger.LogInformation("User data sent");
        return Ok();
    }


    [HttpGet("GetWebSocket")]
    public async Task GetWebSocket()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _webSockets.Add(webSocket);
            _logger.LogInformation("WebSocket connection established.");

            await WaitForWebSocketClose(webSocket);
        }
        else
        {
            _logger.LogWarning("WebSocket connection request failed.");
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private async Task WaitForWebSocketClose(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;

        do
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        } while (!result.CloseStatus.HasValue);

        _webSockets.Remove(webSocket);
        _logger.LogInformation("WebSocket connection closed.");
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _channel?.Close();
                _channel?.Dispose();
                _connection?.Close();
                _connection?.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
