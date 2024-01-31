namespace Models.Models
{
    public class LobbyRequest
    {
        public Guid RequestId { get; set; }
        public GameType GameType { get; set; }  
        public string CallbackUri { get; set; }

    }
}
