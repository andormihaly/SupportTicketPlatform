using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pgvector;
using SupportTicketPlatform.Domain.Entities;
using SupportTicketPlatform.Worker.Application.Configuration;
using SupportTicketPlatform.Worker.Application.Interfaces;
using System.Diagnostics;

namespace SupportTicketPlatform.Worker.Application.Features.Tickets.Commands.AnalyzeTicket;

public sealed class AnalyzeTicketHandler(ITicketRepository ticketRepository, IEmbeddingService embeddingService, ITicketAnalysisService ticketAnalysisService,
    IOptions<WorkerOptions> options, IApplicationTelemetry telemetry, IApplicationMetrics metrics, ILogger<AnalyzeTicketHandler> logger)
{
    public async Task Handle(AnalyzeTicketCommand command, CancellationToken cancellationToken)
    {
        using var activity = telemetry.StartActivity("AnalyzeTicket");

        var startedAt = Stopwatch.GetTimestamp();

        try
        {
            logger.LogInformation("Starting ticket analysis. TicketId: {TicketId}", command.TicketId);

            var ticket = await ticketRepository.GetByIdAsync(command.TicketId, cancellationToken)
                ?? throw new InvalidOperationException($"Ticket '{command.TicketId}' was not found for analysis.");

            var embedding = ticket.Embedding?.ToArray() ?? await embeddingService.GenerateEmbeddingAsync(ticket.Description, cancellationToken);

            var maxResults = options.Value.TicketAnalysis.MaxSimilarTickets;
            IReadOnlyList<Ticket> resolvedTickets;

            using (telemetry.StartActivityWithTags("FindSimilarTickets", new Dictionary<string, object?>
            {
                ["ticket.id"] = ticket.Id,
                ["search.max_results"] = maxResults
            }))
            {
                logger.LogInformation("Searching for similar resolved tickets. TicketId: {TicketId}, MaxResults: {MaxResults}", ticket.Id, maxResults);

                resolvedTickets = await ticketRepository.FindSimilarResolvedTicketsAsync(embedding, maxResults, cancellationToken, ticket.Id);

                logger.LogInformation("Similar ticket search completed. TicketId: {TicketId}, ResultCount: {ResultCount}", ticket.Id, resolvedTickets.Count);
            }

            var analysis = await ticketAnalysisService.GenerateAnalysisAsync(ticket, resolvedTickets, cancellationToken);

            if (string.IsNullOrWhiteSpace(analysis))
                throw new InvalidOperationException("Ticket analysis returned an empty analysis.");

            ticket.Embedding ??= new Vector(embedding);
            ticket.Analysis ??= new TicketAnalysis { TicketId = ticket.Id };
            ticket.Analysis.Content = analysis;

            await ticketRepository.SaveChangesAsync(cancellationToken);

            var durationSeconds = Stopwatch.GetElapsedTime(startedAt).TotalSeconds;
            metrics.TicketAnalysisCompleted(durationSeconds);

            logger.LogInformation("Ticket analysis completed successfully. TicketId: {TicketId}", command.TicketId);
        }
        catch (Exception ex)
        {
            telemetry.RecordException(ex);
            logger.LogError(ex, "Ticket analysis failed. TicketId: {TicketId}", command.TicketId);
            throw;
        }
    }
}
