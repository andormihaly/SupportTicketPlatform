namespace SupportTicketPlatform.Contracts.Messages;

public sealed record TicketAnalysisRequested(
    Guid TicketId,
    DateTimeOffset OccurredAt);
