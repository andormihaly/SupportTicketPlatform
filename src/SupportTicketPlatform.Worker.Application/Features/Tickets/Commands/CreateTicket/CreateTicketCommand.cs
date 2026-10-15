namespace SupportTicketPlatform.Worker.Application.Features.Tickets.Commands.CreateTicket;

public sealed record CreateTicketCommand(
    Guid TicketId,
    string Title,
    string Description,
    string Status,
    DateTimeOffset OccurredAt);
