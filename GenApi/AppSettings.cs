namespace GenApi;

public class AppSettings
{
    public string ConnectionStrings { get; set; } = string.Empty;
    public OpenSearchSettings? OpenSearch { get; set; }
    public RabbitMQSettings? RabbitMQ { get; set; }
}

public class OpenSearchSettings
{
    public string NodeUris { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RabbitMQSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 0;
    public string QueueName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
