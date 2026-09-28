namespace SupportTicketPlatform.Api.Application.Queries.Features.TicketAnalysis.GetTicketAnalysis;

public sealed record TicketAnalysisResponse(
    Guid TicketId,
    string Content,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastModifiedAt);
