using SupportTicketPlatform.Application.Features.Tickets.Queries.GetSimilarTickets;
using SupportTicketPlatform.Application.Features.Tickets.Queries.GetTicket;
using SupportTicketPlatform.Domain.Entities;

namespace SupportTicketPlatform.Application.Interfaces;

public interface ITicketReader
{
    Task<TicketResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<SimilarTicketResponse>?> GetSimilarAsync(Guid id, CancellationToken cancellationToken);

    Task<TicketAnalysis?> GetAnalysisAsync(Guid ticketId,  CancellationToken cancellationToken);
}
