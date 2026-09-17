using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pgvector;

namespace SupportTicketPlatform.Worker.Infrastructure.Persistence.Converters;

public sealed class VectorValueConverter : ValueConverter<float[]?, Vector?>
{
    public VectorValueConverter() : base(
        value => value == null ? null : new Vector(value),
        value => value == null ? null : value.ToArray())
    {
    }
}
