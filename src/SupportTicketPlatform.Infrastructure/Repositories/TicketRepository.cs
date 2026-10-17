using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using SupportTicketPlatform.Application.Features.Tickets.Queries.GetSimilarTickets;
using SupportTicketPlatform.Application.Features.Tickets.Queries.GetTicket;
using SupportTicketPlatform.Application.Interfaces;
using SupportTicketPlatform.Domain.Entities;
using SupportTicketPlatform.Persistence;
using SupportTicketPlatform.Persistence.Redis;
using System.Numerics.Tensors;

namespace SupportTicketPlatform.Infrastructure.Repositories;

public sealed class TicketRepository(AppDbContext dbContext, RedisSimilarityCache similarityCache = null) : ITicketRepository
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
        return await dbContext.TicketAnalyses
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.TicketId == ticketId, cancellationToken);
    }

    public async Task<IReadOnlyList<SimilarTicketResponse>?> GetSimilarAsync(Guid id, int limit, CancellationToken cancellationToken)
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

        return await GetSimilarAsync(source.Embedding.ToArray(), limit, cancellationToken, id);
    }

    public async Task<IReadOnlyList<SimilarTicketResponse>> GetSimilarAsync(float[] embedding, int limit, CancellationToken cancellationToken, Guid? sourceTicketId = null)
    {
        var tickets = sourceTicketId.HasValue  ? await (similarityCache?.GetByIdAsync(sourceTicketId.Value) ?? Task.FromResult<IReadOnlyList<Ticket>?>(null)) : null;

        if (tickets is null && similarityCache is not null)
            tickets = await similarityCache.GetAsync(embedding);

        if (tickets is null)
        {
            tickets = await FindSimilarResolvedTicketsAsync(embedding, limit, cancellationToken, sourceTicketId);

            if (similarityCache is not null)
                await similarityCache.SetAsync(embedding, tickets, sourceTicketId);
        }

        return tickets
            .Select(ticket => new SimilarTicketResponse(
                ticket.Id,
                ticket.Title,
                ticket.Description,
                ticket.Resolution!.Content,
                TensorPrimitives.CosineSimilarity(ticket.Embedding!.ToArray(), embedding)))
            .OrderByDescending(ticket => ticket.Similarity)
            .ThenBy(ticket => ticket.Id)
            .Take(limit)
            .ToList();
    }

    public async Task<IReadOnlyList<Ticket>> FindSimilarResolvedTicketsAsync(float[] embedding, int limit, CancellationToken cancellationToken, Guid? excludeTicketId = null)
    {
        var vector = new Vector(embedding);

        return await dbContext.Tickets
            .AsNoTracking()
            .Include(ticket => ticket.Resolution)
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
