namespace Models.Models
{
    public static class AuthConstants
    {
        public const string ApiKeySectionName = "Authentication:ApiKey";
        public const string ApiKeyHeaderName = "x-api-key";
        public const string MessageBrokerHost = "MessageBroker:Host";
        public const string MessageBrokerUserName = "MessageBroker:UserName";
        public const string MessageBrokerPassword = "MessageBroker:Password";
        public const string DatabaseConnectionString = "SqlConnectionString:Default";
        public const string RabbitMqUri = "RabbitMq:URI";
    }
}
