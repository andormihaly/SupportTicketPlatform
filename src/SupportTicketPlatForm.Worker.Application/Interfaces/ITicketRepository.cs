using SupportTicketPlatform.Worker.Domain.Entities;

namespace SupportTicketPlatform.Worker.Application.Interfaces;

public interface ITicketRepository
{
    Task<bool> ExistsAsync(Guid ticketId, CancellationToken cancellationToken);
    Task AddAsync(Ticket ticket, CancellationToken cancellationToken);
    Task<IReadOnlyList<Ticket>> FindSimilarResolvedTicketsAsync(float[] embedding, int limit, CancellationToken cancellationToken);
}
