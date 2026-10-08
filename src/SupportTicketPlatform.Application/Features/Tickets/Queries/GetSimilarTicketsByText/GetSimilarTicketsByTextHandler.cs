using Microsoft.Extensions.Options;
using SupportTicketPlatform.Application.Configuration;
using SupportTicketPlatform.Application.Features.Tickets.Queries.GetSimilarTickets;
using SupportTicketPlatform.Application.Interfaces;

namespace SupportTicketPlatform.Application.Features.Tickets.Queries.GetSimilarTicketsByText;

public sealed class GetSimilarTicketsByTextHandler(ITicketRepository ticketRepository, IEmbeddingService embeddingService, IOptions<ApiOptions> options)
{
    public async Task<IReadOnlyList<SimilarTicketResponse>> Handle(GetSimilarTicketsByTextQuery query, CancellationToken cancellationToken)
    {
        var embedding = await embeddingService.GenerateEmbeddingAsync(query.Text, cancellationToken);

        return await ticketRepository.GetSimilarAsync(embedding, options.Value.SimilarTickets.MaxResults, cancellationToken);
    }
}
