using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using SupportTicketPlatform.Domain.Entities;
using SupportTicketPlatform.Persistence;
using SupportTicketPlatform.Persistence.Redis;
using SupportTicketPlatform.Worker.Application.Interfaces;

namespace SupportTicketPlatform.Worker.Infrastructure.Repositories;

public sealed class TicketRepository(AppDbContext dbContext, RedisSimilarityCache similarityCache = null) : ITicketRepository
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

    public Task<Ticket?> GetByIdAsync(Guid ticketId, CancellationToken cancellationToken)
    {
        return dbContext.Tickets
            .Include(ticket => ticket.Analysis)
            .SingleOrDefaultAsync(ticket => ticket.Id == ticketId, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }



    public async Task<IReadOnlyList<Ticket>> FindSimilarResolvedTicketsAsync(
     float[] embedding,
     int limit,
     CancellationToken cancellationToken,
     Guid? excludeTicketId = null)
    {
        if (similarityCache is not null)
        {
            if (excludeTicketId.HasValue)
            {
                var cachedTicketsById = await similarityCache.GetByIdAsync(excludeTicketId.Value);

                if (cachedTicketsById is not null)
                    return cachedTicketsById;
            }

            var cachedTickets = await similarityCache.GetAsync(embedding);

            if (cachedTickets is not null)
                return cachedTickets;
        }

        var vector = new Vector(embedding);

        var tickets = await dbContext.Tickets
            .AsNoTracking()
            .Include(x => x.Resolution)
            .Where(ticket => (!excludeTicketId.HasValue || ticket.Id != excludeTicketId.Value)
                && ticket.Status == TicketStatus.Resolved
                && ticket.Resolution != null
                && ticket.Embedding != null)
            .OrderBy(ticket => ticket.Embedding!.CosineDistance(vector))
            .ThenBy(ticket => ticket.Id)
            .Take(limit)
            .ToListAsync(cancellationToken);

        if (similarityCache is not null)
            await similarityCache.SetAsync(embedding, tickets, excludeTicketId);

        return tickets;
    }
}
