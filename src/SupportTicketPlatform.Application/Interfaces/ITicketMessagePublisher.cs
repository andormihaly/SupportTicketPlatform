using SupportTicketPlatform.Contracts.Messages;

namespace SupportTicketPlatform.Application.Messaging;

public interface ITicketMessagePublisher
{
    Task PublishAsync(
        TicketCreated message,
        CancellationToken cancellationToken = default);

    Task PublishAsync(
        TicketAnalysisRequested message,
        CancellationToken cancellationToken = default);
}
