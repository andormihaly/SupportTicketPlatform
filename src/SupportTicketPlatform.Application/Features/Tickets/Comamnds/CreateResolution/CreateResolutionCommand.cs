namespace SupportTicketPlatform.Application.Features.Tickets.Commands.CreateResolution;

public sealed record CreateResolutionCommand(Guid TicketId, string Content);
