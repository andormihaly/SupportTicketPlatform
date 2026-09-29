using Pgvector;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SupportTicketPlatform.Persistence.Redis;

internal sealed class VectorJsonConverter : JsonConverter<Vector>
{
    public override Vector Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var values = JsonSerializer.Deserialize<float[]>(ref reader, options)
            ?? throw new JsonException("Vector value cannot be null.");

        return new Vector(values);
    }

    public override void Write(Utf8JsonWriter writer, Vector value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.ToArray(), options);
    }
}
