using Microsoft.AspNetCore.Mvc;


namespace LobbyManager.Controllers
{
    [ApiController]
    [Route("api/[controller]/lobby")]
    public class LobbyController : Controller
    {
        private readonly ILogger<LobbyController> _logger;


        public LobbyController(ILogger<LobbyController> logger)
        {
            _logger = logger;

        }



        [HttpGet("/GetLobbyName")]
        public string GetLobbyName()
        {
            _logger.LogCritical("critically unimportant");
            return "hi";
        }
    }
}
