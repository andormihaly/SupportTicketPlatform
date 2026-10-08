using Microsoft.Extensions.Logging;
using SupportTicketPlatform.Application.Interfaces;
using SupportTicketPlatform.Application.Messaging;
using SupportTicketPlatform.Contracts.Messages;
using SupportTicketPlatform.Contracts.Tickets;

namespace SupportTicketPlatform.Application.Features.Tickets.Commands.AnalyzeTicket;

public sealed class AnalyzeTicketHandler(ITicketMessagePublisher ticketMessagePublisher, IApplicationTelemetry telemetry, ILogger<AnalyzeTicketHandler> logger)
{
    public async Task Handle(AnalyzeTicketCommand command, CancellationToken cancellationToken)
    {
        using var activity = telemetry.StartActivity("AnalyzeTicket");

        try
        {
            logger.LogInformation("Requesting ticket analysis. TicketId: {TicketId}", command.TicketId);

            var message = new TicketAnalysisRequested(command.TicketId, DateTimeOffset.UtcNow);

            await ticketMessagePublisher.PublishAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            telemetry.RecordException(ex);
            logger.LogError(ex, "Ticket analysis request failed. TicketId: {TicketId}", command.TicketId);
            throw;
        }
    }
}
