using SupportTicketPlatform.Application.Configuration;
using SupportTicketPlatform.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace SupportTicketPlatform.Application.Features.Tickets.Queries.GetSimilarTickets;

public sealed class GetSimilarTicketsHandler(ITicketRepository ticketRepository, IOptions<ApiOptions> options)
{
    public Task<IReadOnlyList<SimilarTicketResponse>?> Handle(GetSimilarTicketsQuery query, CancellationToken cancellationToken)
    {
        return ticketRepository.GetSimilarAsync(query.Id, options.Value.SimilarTickets.MaxResults, cancellationToken);
    }
}
