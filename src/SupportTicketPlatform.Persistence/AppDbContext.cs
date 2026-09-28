using Microsoft.EntityFrameworkCore;
using SupportTicketPlatform.Domain.Common;
using SupportTicketPlatform.Domain.Entities;

namespace SupportTicketPlatform.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options, TimeProvider timeProvider) : DbContext(options)
{
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Resolution> Resolutions => Set<Resolution>();
    public DbSet<TicketAnalysis> TicketAnalyses => Set<TicketAnalysis>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow();

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = now;

            if (entry.State == EntityState.Modified)
                entry.Entity.LastModifiedAt = now;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
