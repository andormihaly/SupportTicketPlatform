using Microsoft.Extensions.Logging;
using Pgvector;
using SupportTicketPlatform.Domain.Entities;
using SupportTicketPlatform.Worker.Application.Interfaces;

namespace SupportTicketPlatform.Worker.Application.Features.Tickets.Commands.CreateTicket;

public sealed class CreateTicketHandler(ITicketRepository ticketRepository, IEmbeddingService embeddingService, IApplicationTelemetry telemetry,
    IApplicationMetrics metrics, ILogger<CreateTicketHandler> logger)
{
    public async Task Handle(CreateTicketCommand command, CancellationToken cancellationToken)
    {
        using var activity = telemetry.StartActivity("CreateTicketPersistence");

        try
        {
            logger.LogInformation("Creating ticket in persistence. TicketId: {TicketId}", command.TicketId);

            if (await ticketRepository.ExistsAsync(command.TicketId, cancellationToken))
            {
                logger.LogInformation("Ticket already exists. Skipping creation. TicketId: {TicketId}", command.TicketId);
                return;
            }

            var status = Enum.Parse<TicketStatus>(command.Status, true);

            var embedding = await embeddingService.GenerateEmbeddingAsync(command.Description, cancellationToken);

            var ticket = new Ticket
            {
                Id = command.TicketId,
                Title = command.Title,
                Description = command.Description,
                Status = status,
                OccurredAt = command.OccurredAt.ToUniversalTime(),
                Embedding = new Vector(embedding)
            };

            await ticketRepository.AddAsync(ticket, cancellationToken);

            metrics.TicketCreated();

            logger.LogInformation("Ticket persisted successfully. TicketId: {TicketId}", command.TicketId);
        }
        catch (Exception ex)
        {
            telemetry.RecordException(ex);
            logger.LogError(ex, "Ticket creation failed. TicketId: {TicketId}", command.TicketId);
            throw;
        }
    }
}
