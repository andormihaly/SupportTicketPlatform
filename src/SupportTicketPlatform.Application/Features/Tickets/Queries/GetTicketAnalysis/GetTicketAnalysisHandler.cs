using SupportTicketPlatform.Application.Interfaces;

namespace SupportTicketPlatform.Application.Queries.Features.TicketAnalysis.GetTicketAnalysis;

public sealed class GetTicketAnalysisHandler(ITicketRepository ticketRepository)
{
    public async Task<TicketAnalysisResponse?> Handle(GetTicketAnalysisQuery query, CancellationToken cancellationToken)
    {
        var analysis = await ticketRepository.GetAnalysisAsync(query.TicketId, cancellationToken);

        if (analysis is null)
            return null;

        return new TicketAnalysisResponse(analysis.TicketId, analysis.Content, analysis.CreatedAt, analysis.LastModifiedAt);
    }
}
