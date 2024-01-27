namespace Models.Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public AccessLevel AccessLevel { get; set; }    
        public string UserName { get; set; } = string.Empty;    
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }   
        public double Rating { get; set; }  
        public double Currency { get; set; }    

    }
}
