using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTicketPlatform.Worker.Domain.Entities;

namespace SupportTicketPlatform.Worker.Infrastructure.Persistence.Configurations;

public sealed class ResolutionConfiguration : IEntityTypeConfiguration<Resolution>
{
    public void Configure(EntityTypeBuilder<Resolution> builder)
    {
        builder.ToTable("Resolutions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content).HasMaxLength(4000).IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.Property(x => x.LastModifiedAt);

        builder.HasOne(x => x.Ticket).WithOne(x => x.Resolution).HasForeignKey<Resolution>(x => x.TicketId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.TicketId).IsUnique();
    }
}
