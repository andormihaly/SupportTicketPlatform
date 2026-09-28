using SupportTicketPlatform.Application.Interfaces;

namespace SupportTicketPlatform.Application.Features.Tickets.Queries.GetTicket;

public sealed class GetTicketHandler(ITicketRepository ticketRepository)
{
    public Task<TicketResponse?> Handle(GetTicketQuery query, CancellationToken cancellationToken)
    {
        return ticketRepository.GetDetailsAsync(query.Id, cancellationToken);
    }
}
