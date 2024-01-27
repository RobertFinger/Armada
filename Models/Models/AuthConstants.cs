namespace Models.Models
{
    public static class AuthConstants
    {
        public const string ApiKeySectionName = "Authentication:ApiKey";
        public const string ApiKeyHeaderName = "x-api-key";
        public const string MessageBrokerHost = "MessageBroker:Host";
        public const string MessageBrokerUserName = "MessageBroker:UserName";
        public const string MessageBrokerPassword = "MessageBroker:Password";
    }

    public static class MessengerConstants 
    {
        public const string FactoryUri = "MessengerFactory:Uri";
        public const string ClientProviderName = "MessengerFactory:ClientProviderName";
        public const string ExchangeName = "MessengerFactory:ExchangeName";
        public const string RoutingKey = "MessengerFactory:RoutingKey";
        public const string QueueNameAM = "MessengerFactory:AccountsManager";
        public const string QueueNameDM = "MessengerFactory:DataManager";
        public const string QueueNameGM = "MessengerFactory:GameManager";
        public const string QueueNameLM = "MessengerFactory:LobbyManager";
        public const string QueueNameDevPortal = "MessengerFactory:DeveloperPortal";
    }
}
