using Pgvector;
using SupportTicketPlatform.Domain.Common;

namespace SupportTicketPlatform.Domain.Entities;

public sealed class Ticket : AuditableEntity
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public TicketStatus Status { get; set; } = TicketStatus.Open;

    public DateTimeOffset OccurredAt { get; set; }
    public Resolution? Resolution { get; set; }

    public Vector? Embedding { get; set; }
}
