namespace SupportTicketPlatform.Application.Configuration;

public sealed class ApiOptions
{
    public ConnectionStringsOptions ConnectionStrings { get; set; } = new();
    public ServiceBusOptions ServiceBus { get; set; } = new();
    public FoundryOptions Foundry { get; set; } = new();

    public SimilarTicketsOptions SimilarTickets { get; set; } = new();
}

public sealed class ConnectionStringsOptions
{
    public string PostgreSQLConnectionString { get; set; } = string.Empty;
    public string RedisConnectionString { get; set; } = string.Empty;
}

public sealed class ServiceBusOptions
{
    public string FullyQualifiedNamespace { get; set; } = string.Empty;
}

public sealed class FoundryOptions
{
    public string ProjectEndpoint { get; set; } = string.Empty;
    public string OpenAIEndpoint { get; set; } = string.Empty;
    public string EmbeddingDeployment { get; set; } = string.Empty;
    public string TicketAnalysisDeployment { get; set; } = string.Empty;
}

public sealed class SimilarTicketsOptions
{
    public int MaxResults { get; set; }
}


