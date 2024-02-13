using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;

namespace DataManager.Controllers
{
    [ApiController]
    [Route("api/[controller]/Data"), Authorize]
    public class DataController : Controller
    {
        private readonly ILogger<DataController> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public DataController(ILogger<DataController> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        [HttpPost("/SaveUserData/{id}")]
        public async Task MessengerTest(User user)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _logger.LogInformation("Add user to database {user}", user);

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }

        [HttpGet("/GetLobby/")]
        public async Task<List<LobbyGameData>> GetLobby(Guid lobby)
        {

            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _logger.LogInformation("Provide requested lobby {lobby}", lobby);

            var lobbyItems = dbContext.LobbyGameData.Where(i => i.Id == lobby).ToList() ?? new List<LobbyGameData>();
            var maxLobbyItems = 9;

            var currentItems = lobbyItems.Count;
            if (currentItems < maxLobbyItems)
            {
                Enumerable.Range(currentItems, maxLobbyItems - currentItems).ToList().ForEach(_ =>
                    lobbyItems.Add(new LobbyGameData(lobby)));
            }

            dbContext.LobbyGameData.AddRange(lobbyItems);
            await dbContext.SaveChangesAsync();

            return lobbyItems;
        }

        [HttpPost("/AddUserToGame/{id}")]
        public async Task AddUserToGame(Guid lobby, Guid game, Guid player)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _logger.LogInformation("Add user to game: {game} player:{player}, lobby:{lobby}", game, player, lobby);

            //dbContext.Users.Add(user);
            //await dbContext.SaveChangesAsync();
        }

    }
}
