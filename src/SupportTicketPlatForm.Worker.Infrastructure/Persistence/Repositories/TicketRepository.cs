using Microsoft.EntityFrameworkCore;
using Pgvector;
using SupportTicketPlatform.Worker.Application.Interfaces;
using SupportTicketPlatform.Worker.Domain.Entities;

namespace SupportTicketPlatform.Worker.Infrastructure.Persistence.Repositories;

public sealed class TicketRepository(AppDbContext dbContext) : ITicketRepository
{
    public Task<bool> ExistsAsync(Guid ticketId, CancellationToken cancellationToken)
    {
        return dbContext.Tickets.AnyAsync(x => x.Id == ticketId, cancellationToken);
    }

    public async Task AddAsync(Ticket ticket, CancellationToken cancellationToken)
    {
        dbContext.Tickets.Add(ticket);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Ticket>> FindSimilarResolvedTicketsAsync(float[] embedding, int limit, CancellationToken cancellationToken)
    {
        var vector = new Vector(embedding);

        return await dbContext.Tickets
            .FromSqlRaw("""
            SELECT t.*
            FROM "Tickets" t
            WHERE t."Embedding" IS NOT NULL
            ORDER BY t."Embedding" <=> {0}
            LIMIT {1}
            """, vector, limit)
            .AsNoTracking()
            .Include(x => x.Resolution)
            .ToListAsync(cancellationToken);
    }
}
