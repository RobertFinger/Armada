using Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using System.Text;
using System.Text.Json;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("api/[controller]/Gateway")]
    public class GatewayController : Controller
    {
        private readonly ILogger<GatewayController> _logger;
        private readonly GatewaySender _messages;

        public GatewayController(ILogger<GatewayController> logger, GatewaySender messages)
        {
            _logger = logger;
            _messages = messages;
        }

        [HttpGet("/MessengerTest")]
        public void MessengerTest()
        {
            var mesageForAccountsManager = "Gateway to Accounts Manager, come in Accounts Manager!";
            var mesageForDataManager = "Gateway to Data Manager, come in Data Manager!";
            var mesageForGameManager = "Gateway to LobbyGameData Manager, come in LobbyGameData Manager!";
            var mesageForlobbyManager = "Gateway to Lobby Manager, come in Lobby Manager!";
            var mesageForPaymentManager = "Gateway to Payment Manager, come in Payment Manager!";

            _messages.SendMessage(MessageDestination.Accounts, mesageForAccountsManager);
            _messages.SendMessage(MessageDestination.Data, mesageForDataManager);
            _messages.SendMessage(MessageDestination.Game, mesageForGameManager);
            _messages.SendMessage(MessageDestination.Lobby, mesageForlobbyManager);
            _messages.SendMessage(MessageDestination.Payments, mesageForPaymentManager);
        }

        [HttpGet("/GetLobby"), Authorize(Roles = "User")]
        public ActionResult<Guid> GetLobby()
        {
            var lobbyRequest = new LobbyRequest() { RequestId = Guid.NewGuid(), GameType = GameType.Numbers, CallbackUri = "https://www.google.com"};
            var requestJson = JsonSerializer.Serialize(lobbyRequest);

            _messages.SendMessage(MessageDestination.Lobby, requestJson);     
            _logger.LogInformation("lobby request");
            
            return Ok(lobbyRequest.RequestId);
        }
    }
}
