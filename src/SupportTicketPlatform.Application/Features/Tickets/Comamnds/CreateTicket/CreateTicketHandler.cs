using SupportTicketPlatform.Application.Interfaces;
using SupportTicketPlatform.Domain.Entities;

namespace SupportTicketPlatform.Application.Features.Tickets.Commands.CreateTicket;

public sealed class CreateTicketHandler()
{
    public async Task<Guid> Handle(CreateTicketCommand command, CancellationToken cancellationToken)
    {

        return Guid.NewGuid();
    }
}
