using System.ComponentModel.DataAnnotations;

namespace Models.Models;

public class UserProfile
{
    [Key]
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; }   
    public Uri TwitchLink { get; set; } 
    public int Wins {  get; set; }  
    public int Losses { get; set; } 
    public List<UserProfile> Friends { get; set; }
}
