using Microsoft.EntityFrameworkCore;
using SupportTicketPlatform.Application.Features.Tickets.Queries.GetTicket;
using SupportTicketPlatform.Application.Interfaces;
using SupportTicketPlatform.Persistence;

namespace SupportTicketPlatform.Infrastructure.Repositories;

public sealed class TicketReader(AppDbContext dbContext) : ITicketReader
{
    public Task<TicketResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Tickets
            .AsNoTracking()
            .Where(ticket => ticket.Id == id)
            .Select(ticket => new TicketResponse(
                ticket.Id,
                ticket.Title,
                ticket.Description,
                ticket.Status,
                ticket.OccurredAt,
                ticket.CreatedAt,
                ticket.Resolution == null ? null : new ResolutionResponse(ticket.Resolution.Content)))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
