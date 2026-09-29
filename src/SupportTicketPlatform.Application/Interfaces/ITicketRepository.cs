using SupportTicketPlatform.Application.Features.Tickets.Queries.GetSimilarTickets;
using SupportTicketPlatform.Application.Features.Tickets.Queries.GetTicket;
using SupportTicketPlatform.Domain.Entities;

namespace SupportTicketPlatform.Application.Interfaces;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<TicketResponse?> GetDetailsAsync(Guid id, CancellationToken cancellationToken);

    Task<TicketAnalysis?> GetAnalysisAsync(Guid ticketId, CancellationToken cancellationToken);

    Task<IReadOnlyList<SimilarTicketResponse>?> GetSimilarAsync(Guid id, int limit, CancellationToken cancellationToken);

    Task<IReadOnlyList<SimilarTicketResponse>> GetSimilarAsync(float[] embedding, int limit, CancellationToken cancellationToken, Guid? sourceTicketId = null);

    Task<IReadOnlyList<Ticket>> FindSimilarResolvedTicketsAsync(float[] embedding, int limit, CancellationToken cancellationToken, Guid? excludeTicketId = null);
}
