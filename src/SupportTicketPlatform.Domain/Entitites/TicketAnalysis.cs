using SupportTicketPlatform.Domain.Common;

namespace SupportTicketPlatform.Domain.Entities;

public sealed class TicketAnalysis : AuditableEntity
{
    public Guid TicketId { get; set; }

    public string Content { get; set; } = string.Empty;

    public Ticket Ticket { get; set; } = null!;
}
