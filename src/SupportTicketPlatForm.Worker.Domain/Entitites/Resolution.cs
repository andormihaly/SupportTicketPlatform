using SupportTicketPlatform.Worker.Domain.Common;

namespace SupportTicketPlatform.Worker.Domain.Entities;

public sealed class Resolution : AuditableEntity
{
    public Guid TicketId { get; set; }

    public string Content { get; set; } = string.Empty;

    public Ticket Ticket { get; set; } = null!;
}
