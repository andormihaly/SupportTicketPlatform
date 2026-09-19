using SupportTicketPlatform.Worker.Application.Interfaces;
using SupportTicketPlatform.Worker.Domain.Entities;

namespace SupportTicketPlatform.Worker.Application.Features.Tickets.Commands.CreateTicket;

public sealed class CreateTicketHandler(ITicketRepository ticketRepository, IEmbeddingService embeddingService)
{
    public async Task Handle(CreateTicketCommand command, CancellationToken cancellationToken)
    {
        if (await ticketRepository.ExistsAsync(command.TicketId, cancellationToken))
            return;

        var status = Enum.Parse<TicketStatus>(command.Status, true);
        var embedding = await embeddingService.GenerateEmbeddingAsync(command.Description, cancellationToken);

        var ticket = new Ticket
        {
            Id = command.TicketId,
            Title = command.Title,
            Description = command.Description,
            Status = status,
            OccurredAt = command.OccurredAt.ToUniversalTime(),
            Embedding = embedding
        };

        await ticketRepository.AddAsync(ticket, cancellationToken);
    }

}
