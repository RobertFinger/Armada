

namespace Models.Models
{
    public class LobbyGameData
    {
        public LobbyGameData(IEnumerable<Player>? players, int maxPlayers)
        {
            Players = players;
            Id = Guid.NewGuid(); 
            Created = DateTime.UtcNow;
            MaxPlayers = maxPlayers;
        }


        public LobbyGameData()
        {
            Players = new List<Player>();
            Id = Guid.NewGuid();
            Created = DateTime.UtcNow;
            MaxPlayers = 2;
        }

        public Guid LobbyId { get; set; }
        public Guid Id { get;}
        public IEnumerable<Player>? Players { get; set; }
        public int MaxPlayers { get; set; }
        public bool Complete { get; set; }
        public Guid? Winner { get; set; }
        public GameAccessType GameAccessType { get; set; }
        public GameType GameType { get; set; }
        public DateTime? Created { get; }
        public DateTime StartTime { get; set; } 
        public DateTime EndTime { get; set; } 

    }
}
