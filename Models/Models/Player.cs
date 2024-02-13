using System.ComponentModel.DataAnnotations;

namespace Models.Models
{
    public class Player
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public AccessLevel AccessLevel { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public double Rating { get; set; }
        public double Currency { get; set; }

    }
}
