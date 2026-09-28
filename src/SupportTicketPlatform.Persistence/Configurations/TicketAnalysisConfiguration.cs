using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTicketPlatform.Domain.Entities;

namespace SupportTicketPlatform.Persistence.Configurations;

public sealed class TicketAnalysisConfiguration : IEntityTypeConfiguration<TicketAnalysis>
{
    public void Configure(EntityTypeBuilder<TicketAnalysis> builder)
    {
        builder.ToTable("TicketAnalyses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content).HasColumnType("text").IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.Property(x => x.LastModifiedAt);

        builder.HasOne(x => x.Ticket).WithOne(x => x.Analysis).HasForeignKey<TicketAnalysis>(x => x.TicketId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.TicketId).IsUnique();
    }
}
