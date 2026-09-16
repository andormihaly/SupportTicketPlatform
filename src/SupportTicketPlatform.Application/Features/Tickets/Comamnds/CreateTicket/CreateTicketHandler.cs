using SupportTicketPlatform.Application.Messaging;
using SupportTicketPlatform.Contracts.Messages;
using SupportTicketPlatform.Domain.Entities;

namespace SupportTicketPlatform.Application.Features.Tickets.Commands.CreateTicket;

public sealed class CreateTicketHandler(ITicketMessagePublisher ticketMessagePublisher)
{
    public async Task<Guid> Handle(CreateTicketCommand command, CancellationToken cancellationToken)
    {
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            Description = command.Description,
            Status = TicketStatus.Open,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var message = new TicketCreated(ticket.Id, ticket.Title, ticket.Description, ticket.Status.ToString(), ticket.CreatedAt);

        await ticketMessagePublisher.PublishAsync(message, cancellationToken);

        return ticket.Id;
    }
}
