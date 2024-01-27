using Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using System.Text;

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

        [HttpGet("/Messenger")]
        public void Messenger()
        {
            var mesageForAccountsManager = Encoding.UTF8.GetBytes("Gateway to Accounts Manager, come in Accounts Manager!");
            var mesageForDataManager = Encoding.UTF8.GetBytes("Gateway to Data Manager, come in Data Manager!");
            var mesageForGameManager = Encoding.UTF8.GetBytes("Gateway to Game Manager, come in Game Manager!");
            var mesageForlobbyManager = Encoding.UTF8.GetBytes("Gateway to Lobby Manager, come in Lobby Manager!");
            var mesageForPaymentManager = Encoding.UTF8.GetBytes("Gateway to Payment Manager, come in Payment Manager!");

            _messages.SendMessage(MessageDestination.Accounts, mesageForAccountsManager);
            _messages.SendMessage(MessageDestination.Data, mesageForDataManager);
            _messages.SendMessage(MessageDestination.Game, mesageForGameManager);
            _messages.SendMessage(MessageDestination.Lobby, mesageForlobbyManager);
            _messages.SendMessage(MessageDestination.Payments, mesageForPaymentManager);
        }

        [HttpGet("/GetLobby"), Authorize(Roles = "User")]
        public string GetLobby()
        {
 


            return "My Lobby";
        }
    }
}
