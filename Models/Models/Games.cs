using System.ComponentModel.DataAnnotations;

namespace Models.Models;

public class Game
{
    [Key]
    public Guid Id { get; set; }
    public List<Player> Players { get; set; }
    public List<ChallengeWord> ChallengeWords { get; set; } 
    public int MaxLetters { get; set; }
    public int MinLetters { get; set; }
    public int Guesses { get; set; }
}
