using SupportTicketPlatform.Application.Interfaces;

namespace SupportTicketPlatform.Api.Application.Queries.Features.TicketAnalysis.GetTicketAnalysis;

public sealed class GetTicketAnalysisHandler(ITicketReader ticketReader)
{
    public async Task<TicketAnalysisResponse?> Handle(GetTicketAnalysisQuery query, CancellationToken cancellationToken)
    {
        var analysis = await ticketReader.GetAnalysisAsync(query.TicketId, cancellationToken);

        if (analysis is null)
            return null;

        return new TicketAnalysisResponse(analysis.TicketId, analysis.Content, analysis.CreatedAt, analysis.LastModifiedAt);
    }
}
