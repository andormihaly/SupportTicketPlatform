using SupportTicketPlatform.Application.Interfaces;

namespace SupportTicketPlatform.Application.Features.Tickets.Queries.GetSimilarTickets;

public sealed class GetSimilarTicketsHandler(ITicketRepository ticketRepository)
{
    public Task<IReadOnlyList<SimilarTicketResponse>?> Handle(GetSimilarTicketsQuery query, CancellationToken cancellationToken)
    {
        return ticketRepository.GetSimilarAsync(query.Id, cancellationToken);
    }
}
