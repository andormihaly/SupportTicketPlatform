using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTicketPlatform.Worker.Domain.Entities;
using SupportTicketPlatform.Worker.Infrastructure.Persistence.Converters;

namespace SupportTicketPlatForm.Worker.Infrastructure.Persistence.Configurations;

public sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();

        builder.Property(x => x.Description).HasMaxLength(4000).IsRequired();

        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50).IsRequired();

        builder.Property(x => x.OccurredAt).IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.Property(x => x.LastModifiedAt);

        builder.Property(x => x.Embedding).HasConversion<VectorValueConverter>().HasColumnType("vector(1536)");
    }
}
