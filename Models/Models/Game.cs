using System.Collections;

namespace Models.Models
{
    public class Game
    {
        public Game(IEnumerable<Player>? players)
        {
            Players = players;
            Id = Guid.NewGuid(); 
            Created = DateTime.UtcNow;
        }

        public Guid Id { get;}
        public IEnumerable<Player>? Players { get; set; }
        public bool Complete { get; set; }
        public Guid? Winner { get; set; }
        public GameType GameType { get; set; }
        public DateTime? Created { get; }
        public DateTime StartTime { get; set; } 
        public DateTime EndTime { get; set; } 

    }
}
