namespace SupportTicketPlatform.Worker.Application.Features.Tickets.Commands.AnalyzeTicket;

public sealed record AnalyzeTicketCommand(
    Guid TicketId,
    DateTimeOffset OccurredAt);
