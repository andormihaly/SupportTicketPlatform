using SupportTicketPlatform.Application.Messaging;
using SupportTicketPlatform.Contracts.Messages;

namespace SupportTicketPlatform.Application.Features.Tickets.Commands.AnalyzeTicket;

public sealed class AnalyzeTicketHandler(ITicketMessagePublisher ticketMessagePublisher)
{
    public async Task Handle(AnalyzeTicketCommand command, CancellationToken cancellationToken)
    {
        var message = new TicketAnalysisRequested(command.TicketId, DateTimeOffset.UtcNow);

        await ticketMessagePublisher.PublishAsync(message, cancellationToken);
    }
}
