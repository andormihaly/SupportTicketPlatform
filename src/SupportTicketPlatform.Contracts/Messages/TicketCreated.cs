


namespace SupportTicketPlatform.Contracts.Messages;

public sealed record TicketCreated(
    Guid TicketId,
    string Title,
    string Description,
    string Status,
    DateTimeOffset CreatedAt);
