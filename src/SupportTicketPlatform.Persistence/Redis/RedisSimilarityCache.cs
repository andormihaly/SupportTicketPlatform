using StackExchange.Redis;
using SupportTicketPlatform.Domain.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SupportTicketPlatform.Persistence.Redis;

public sealed class RedisSimilarityCache(IConnectionMultiplexer redis, RedisOptions options)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        Converters =
        {
            new VectorJsonConverter()
        }
    };

    public async Task<IReadOnlyList<Ticket>?> GetAsync(float[] embedding)
    {
        var database = redis.GetDatabase();
        var embeddingBytes = ToBytes(embedding);

        var result = await database.ExecuteAsync(
            "FT.SEARCH",
            RedisSimilarityCacheIndex.IndexName,
            "*=>[KNN 1 @embedding $vector AS distance]",
            "PARAMS", 2, "vector", embeddingBytes,
            "SORTBY", "distance",
            "RETURN", 2, RedisSimilarityCacheIndex.ResultField, "distance",
            "LIMIT", 0, 1,
            "DIALECT", 2);

        var response = result.ToDictionary();

        var results = (RedisResult[]?)response["results"];

        if (results is null || results.Length == 0)
            return null;

        var fields = results[0].ToDictionary()["extra_attributes"].ToDictionary();

        var distance = (double)fields["distance"];
        var similarity = 1 - distance;

        if (similarity < options.SimilarityThreshold)
            return null;

        var json = (string?)fields[RedisSimilarityCacheIndex.ResultField];

        if (string.IsNullOrWhiteSpace(json))
            return null;

        return JsonSerializer.Deserialize<List<Ticket>>(json, JsonOptions);
    }

    public async Task<IReadOnlyList<Ticket>?> GetByIdAsync(Guid id)
    {
        var database = redis.GetDatabase();
        var key = $"{RedisSimilarityCacheIndex.KeyPrefix}{id:N}";

        var json = await database.HashGetAsync(key, RedisSimilarityCacheIndex.ResultField);

        if (json.IsNullOrEmpty)
            return null;

        return JsonSerializer.Deserialize<List<Ticket>>(json.ToString(), JsonOptions);
    }

    public async Task SetAsync(float[] embedding, IReadOnlyList<Ticket> tickets, Guid? sourceTicketId = null)
    {
        var database = redis.GetDatabase();

        var id = sourceTicketId ?? Guid.NewGuid();
        var key = $"{RedisSimilarityCacheIndex.KeyPrefix}{id:N}";
        var result = JsonSerializer.Serialize(tickets, JsonOptions);
        var embeddingBytes = ToBytes(embedding);

        await database.HashSetAsync(key,
        [
            new HashEntry(RedisSimilarityCacheIndex.EmbeddingField, embeddingBytes),
        new HashEntry(RedisSimilarityCacheIndex.ResultField, result)
        ]);

        await database.KeyExpireAsync(key, TimeSpan.FromMinutes(options.CacheLifetimeMinutes));
    }

    private static byte[] ToBytes(float[] embedding)
    {
        var bytes = new byte[embedding.Length * sizeof(float)];
        Buffer.BlockCopy(embedding, 0, bytes, 0, bytes.Length);

        return bytes;
    }
}
