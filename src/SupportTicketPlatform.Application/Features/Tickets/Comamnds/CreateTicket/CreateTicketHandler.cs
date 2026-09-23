using SupportTicketPlatform.Application.Messaging;
using SupportTicketPlatform.Contracts.Messages;
using SupportTicketPlatform.Domain.Entities;

namespace SupportTicketPlatform.Application.Features.Tickets.Commands.CreateTicket;

public sealed class CreateTicketHandler(ITicketMessagePublisher ticketMessagePublisher)
{
    public async Task<Guid> Handle(CreateTicketCommand command, CancellationToken cancellationToken)
    {
        var ticketId = Guid.NewGuid();
        var occurredAt = DateTimeOffset.UtcNow;

        var message = new TicketCreated(ticketId, command.Title, command.Description, TicketStatus.Open.ToString(), occurredAt);

        await ticketMessagePublisher.PublishAsync(message, cancellationToken);

        return ticketId;
    }
}
