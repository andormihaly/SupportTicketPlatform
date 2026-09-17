using Microsoft.EntityFrameworkCore;
using SupportTicketPlatform.Worker.Domain.Entities;
using SupportTicketPlatform.Worker.Application.Interfaces;
using SupportTicketPlatform.Worker.Domain.Common;

namespace SupportTicketPlatform.Worker.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options, TimeProvider timeProvider): DbContext(options), IAppDbContext
{
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Resolution> Resolutions => Set<Resolution>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
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
