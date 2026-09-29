using StackExchange.Redis;

namespace SupportTicketPlatform.Persistence.Redis;

public static class RedisSimilarityCacheIndex
{
    public const string IndexName = "tickets-similarity-index";
    public const string KeyPrefix = "tickets:similar:";
    public const string EmbeddingField = "embedding";
    public const string ResultField = "result";
    public const int EmbeddingDimensions = 1536;

    public static async Task CreateAsync(IConnectionMultiplexer redis)
    {
        var database = redis.GetDatabase();

        var indexes = (RedisResult[]?)await database.ExecuteAsync("FT._LIST");

        if (indexes?.Any(index => (string?)index == IndexName) == true)
            return;

        await database.ExecuteAsync(
            "FT.CREATE",
            IndexName,
            "ON", "HASH",
            "PREFIX", 1, KeyPrefix,
            "SCHEMA",
            EmbeddingField, "VECTOR", "FLAT", 6,
            "TYPE", "FLOAT32",
            "DIM", EmbeddingDimensions,
            "DISTANCE_METRIC", "COSINE");
    }
}
