using SupportTicketPlatform.Domain.Entities;

namespace SupportTicketPlatform.Application.Features.Tickets.Queries.GetTicket;

public sealed record TicketResponse(
    Guid Id,
    string Title,
    string Description,
    TicketStatus Status,
    DateTimeOffset OccurredAt,
    DateTimeOffset CreatedAt,
    ResolutionResponse? Resolution);
