using SupportTicketPlatform.Application.Features.Tickets.Queries.GetSimilarTickets;
using SupportTicketPlatform.Application.Interfaces;

namespace SupportTicketPlatform.Application.Features.Tickets.Queries.GetSimilarTicketsByText;

public sealed class GetSimilarTicketsByTextHandler(ITicketRepository ticketRepository, IEmbeddingService embeddingService)
{
    public async Task<IReadOnlyList<SimilarTicketResponse>> Handle(GetSimilarTicketsByTextQuery query, CancellationToken cancellationToken)
    {
        var embedding = await embeddingService.GenerateEmbeddingAsync(query.Text, cancellationToken);

        return await ticketRepository.GetSimilarAsync(embedding,3, cancellationToken);
    }
}
