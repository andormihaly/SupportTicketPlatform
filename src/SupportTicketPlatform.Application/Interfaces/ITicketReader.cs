using SupportTicketPlatform.Application.Features.Tickets.Queries.GetTicket;
using SupportTicketPlatform.Application.Features.Tickets.Queries.GetSimilarTickets;

namespace SupportTicketPlatform.Application.Interfaces;

public interface ITicketReader
{
    Task<TicketResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<SimilarTicketResponse>?> GetSimilarAsync(Guid id, CancellationToken cancellationToken);
}
