using SupportTicketPlatform.Application.Interfaces;
using SupportTicketPlatform.Domain.Entities;

namespace SupportTicketPlatform.Application.Features.Tickets.Commands.CreateResolution;

public sealed class CreateResolutionHandler(ITicketRepository ticketRepository)
{
    public async Task<CreateResolutionResult> Handle(CreateResolutionCommand command, CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);

        if (ticket is null)
            return CreateResolutionResult.TicketNotFound;

        if (ticket.Resolution is not null)
            return CreateResolutionResult.ResolutionAlreadyExists;

        ticket.Resolution = new Resolution
        {
            TicketId = ticket.Id,
            Content = command.Content
        };
        ticket.Status = TicketStatus.Resolved;

        await ticketRepository.SaveChangesAsync(cancellationToken);

        return CreateResolutionResult.Created;
    }
}
