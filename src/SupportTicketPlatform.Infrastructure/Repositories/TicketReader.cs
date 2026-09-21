using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using SupportTicketPlatform.Application.Features.Tickets.Queries.GetSimilarTickets;
using SupportTicketPlatform.Application.Features.Tickets.Queries.GetTicket;
using SupportTicketPlatform.Application.Interfaces;
using SupportTicketPlatform.Domain.Entities;
using SupportTicketPlatform.Persistence;

namespace SupportTicketPlatform.Infrastructure.Repositories;

public sealed class TicketReader(AppDbContext dbContext) : ITicketReader
{
    public Task<TicketResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
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

    public async Task<IReadOnlyList<SimilarTicketResponse>?> GetSimilarAsync(Guid id, CancellationToken cancellationToken)
    {
        var source = await dbContext.Tickets
            .Where(ticket => ticket.Id == id)
            .Select(ticket => new { ticket.Embedding })
            .SingleOrDefaultAsync(cancellationToken);

        if (source is null)
            return null;

        if (source.Embedding is null)
            return [];

        return await dbContext.Tickets
            .AsNoTracking()
            .Where(ticket => ticket.Id != id
                && ticket.Status == TicketStatus.Resolved
                && ticket.Resolution != null
                && ticket.Embedding != null)
            .OrderBy(ticket => ticket.Embedding!.CosineDistance(source.Embedding))
            .ThenBy(ticket => ticket.Id)
            .Take(5)
            .Select(ticket => new SimilarTicketResponse(
                ticket.Id,
                ticket.Title,
                ticket.Description,
                ticket.Resolution!.Content,
                1 - ticket.Embedding!.CosineDistance(source.Embedding)))
            .ToListAsync(cancellationToken);
    }
}
