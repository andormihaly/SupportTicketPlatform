using Pgvector;
using SupportTicketPlatform.Domain.Entities;
using SupportTicketPlatform.Worker.Application.Interfaces;

namespace SupportTicketPlatform.Worker.Application.Features.Tickets.Commands.AnalyzeTicket;

public sealed class AnalyzeTicketHandler(
    ITicketRepository ticketRepository,
    IEmbeddingService embeddingService,
    ITicketAnalysisService ticketAnalysisService)
{
    public async Task Handle(AnalyzeTicketCommand command, CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdAsync(command.TicketId, cancellationToken)
            ?? throw new InvalidOperationException($"Ticket '{command.TicketId}' was not found for analysis.");

        var embedding = ticket.Embedding?.ToArray()
            ?? await embeddingService.GenerateEmbeddingAsync(ticket.Description, cancellationToken);

        var resolvedTickets = await ticketRepository.FindSimilarResolvedTicketsAsync(
            embedding, 5, cancellationToken, ticket.Id);

        var analysis = await ticketAnalysisService.GenerateAnalysisAsync(ticket, resolvedTickets, cancellationToken);

        if (string.IsNullOrWhiteSpace(analysis))
            throw new InvalidOperationException("Ticket analysis returned an empty analysis.");

        ticket.Embedding ??= new Vector(embedding);
        ticket.Analysis ??= new TicketAnalysis { TicketId = ticket.Id };
        ticket.Analysis.Content = analysis;

        await ticketRepository.SaveChangesAsync(cancellationToken);
    }
}
