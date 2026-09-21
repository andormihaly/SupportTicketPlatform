namespace SupportTicketPlatform.Application.Features.Tickets.Queries.GetSimilarTickets;

public sealed record SimilarTicketResponse(
    Guid Id,
    string Title,
    string Description,
    string ResolutionContent,
    double Similarity);
