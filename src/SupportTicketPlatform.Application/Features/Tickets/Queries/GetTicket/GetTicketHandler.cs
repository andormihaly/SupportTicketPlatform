using SupportTicketPlatform.Application.Interfaces;

namespace SupportTicketPlatform.Application.Features.Tickets.Queries.GetTicket;

public sealed class GetTicketHandler(ITicketReader ticketReader)
{
    public Task<TicketResponse?> Handle(GetTicketQuery query, CancellationToken cancellationToken)
    {
        return ticketReader.GetByIdAsync(query.Id, cancellationToken);
    }
}
