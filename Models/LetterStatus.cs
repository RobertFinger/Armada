using System.ComponentModel.DataAnnotations;

namespace Models.Models;

public class LetterStatus
{
    [Key]
    public Guid id { get; set; }
    public char Letter { get; set; }
    public LetterColor Color { get; set;}
}
