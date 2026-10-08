using Microsoft.Extensions.Logging;
using SupportTicketPlatform.Application.Interfaces;
using SupportTicketPlatform.Application.Messaging;
using SupportTicketPlatform.Contracts.Messages;
using SupportTicketPlatform.Contracts.Tickets;
using SupportTicketPlatform.Domain.Entities;

namespace SupportTicketPlatform.Application.Features.Tickets.Commands.CreateTicket;

public sealed class CreateTicketHandler(ITicketMessagePublisher ticketMessagePublisher, IApplicationTelemetry telemetry, ILogger<CreateTicketHandler> logger)
{
    public async Task<Guid> Handle(CreateTicketCommand command, CancellationToken cancellationToken)
    {
        using var activity = telemetry.StartActivity("CreateTicket");

        var ticketId = Guid.NewGuid();

        try
        {
            var occurredAt = DateTimeOffset.UtcNow;

            logger.LogInformation("Creating ticket. TicketId: {TicketId}", ticketId);

            var message = new TicketCreated(ticketId, command.Title, command.Description, TicketStatus.Open.ToString(), occurredAt);

            await ticketMessagePublisher.PublishAsync(message, cancellationToken);

            return ticketId;
        }
        catch (Exception ex)
        {
            telemetry.RecordException(ex);
            logger.LogError(ex, "Ticket creation failed. TicketId: {TicketId}", ticketId);
            throw;
        }
    }
}
