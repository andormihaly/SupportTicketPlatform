using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using SupportTicketPlatform.Application.Features.Tickets.Queries.GetSimilarTickets;
using SupportTicketPlatform.Application.Features.Tickets.Queries.GetTicket;
using SupportTicketPlatform.Application.Interfaces;
using SupportTicketPlatform.Domain.Entities;
using SupportTicketPlatform.Persistence;

namespace SupportTicketPlatform.Infrastructure.Repositories;

public sealed class TicketRepository(AppDbContext dbContext) : ITicketRepository
{
    public Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Tickets
            .Include(ticket => ticket.Resolution)
            .SingleOrDefaultAsync(ticket => ticket.Id == id, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<TicketResponse?> GetDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Tickets
            .AsNoTracking()
            .Where(ticket => ticket.Id == id)
            .Select(ticket => new TicketResponse(
                ticket.Id,
                ticket.Title,
                ticket.Description,
                ticket.Status,
                ticket.OccurredAt,
                ticket.CreatedAt,
                ticket.Resolution == null ? null : new ResolutionResponse(ticket.Resolution.Content)))
            .SingleOrDefaultAsync(cancellationToken);
    }
    public async Task<TicketAnalysis?> GetAnalysisAsync(Guid ticketId, CancellationToken cancellationToken)
    {
        return await dbContext.TicketAnalyses.AsNoTracking().SingleOrDefaultAsync(x => x.TicketId == ticketId, cancellationToken);
    }
    public async Task<IReadOnlyList<SimilarTicketResponse>?> GetSimilarAsync(Guid id, CancellationToken cancellationToken)
    {
        var source = await dbContext.Tickets
            .AsNoTracking()
            .Where(ticket => ticket.Id == id)
            .Select(ticket => new { ticket.Embedding })
            .SingleOrDefaultAsync(cancellationToken);

        if (source is null)
            return null;

        if (source.Embedding is null)
            return [];

        var tickets = await FindSimilarResolvedTicketsAsync(source.Embedding.ToArray(), 5, cancellationToken, id);

        return tickets
            .Select(ticket => new SimilarTicketResponse(
                ticket.Id,
                ticket.Title,
                ticket.Description,
                ticket.Resolution!.Content,
                1 - ticket.Embedding!.CosineDistance(source.Embedding)))
            .ToList();
    }



    public async Task<IReadOnlyList<Ticket>> FindSimilarResolvedTicketsAsync(float[] embedding, int limit, CancellationToken cancellationToken, Guid? excludeTicketId = null)
    {
        var vector = new Vector(embedding);

        return await dbContext.Tickets
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
    }
}
