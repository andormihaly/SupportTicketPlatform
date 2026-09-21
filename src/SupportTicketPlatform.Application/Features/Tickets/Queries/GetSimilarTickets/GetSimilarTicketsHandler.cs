using SupportTicketPlatform.Application.Interfaces;

namespace SupportTicketPlatform.Application.Features.Tickets.Queries.GetSimilarTickets;

public sealed class GetSimilarTicketsHandler(ITicketReader ticketReader)
{
    public Task<IReadOnlyList<SimilarTicketResponse>?> Handle(GetSimilarTicketsQuery query, CancellationToken cancellationToken)
    {
        return ticketReader.GetSimilarAsync(query.Id, cancellationToken);
    }
}
