using SupportTicketPlatform.Contracts.Tickets;

namespace SupportTicketPlatform.Worker.Application.Interfaces;

public interface ITicketAnalysisPublisher
{
    Task PublishAsync(TicketReadyForAnalysis message, CancellationToken cancellationToken);
}
