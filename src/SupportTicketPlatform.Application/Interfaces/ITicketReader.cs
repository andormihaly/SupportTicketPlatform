using SupportTicketPlatform.Application.Features.Tickets.Queries.GetTicket;

namespace SupportTicketPlatform.Application.Interfaces;

public interface ITicketReader
{
    Task<TicketResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
