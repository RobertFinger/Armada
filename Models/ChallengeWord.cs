using System.ComponentModel.DataAnnotations;

namespace Models.Models;

public class ChallengeWord
{
    [Key]
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public Guid GameId { get; set; }    
    public string Challenge { get; set; } = string.Empty;
    public IEnumerable<LetterStatus>? LetterColors { get; set; }    
}
