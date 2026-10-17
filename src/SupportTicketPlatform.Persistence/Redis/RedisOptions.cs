namespace SupportTicketPlatform.Persistence.Redis;

public sealed class RedisOptions
{
    public double SimilarityThreshold { get; set; }
    public int CacheLifetimeMinutes { get; set; }

    public bool Enabled;
}
