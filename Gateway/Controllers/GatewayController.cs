using Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public void GetGatewayName()
        {
            _messages.StartMessageService();
        }

        [HttpGet("/GetLobby"), Authorize(Roles = "User")]
        public string GetLobby()
        {
 


            return "My Lobby";
        }
    }
}
