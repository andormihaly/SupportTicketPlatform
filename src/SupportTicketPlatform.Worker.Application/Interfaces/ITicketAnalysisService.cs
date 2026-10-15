using SupportTicketPlatform.Domain.Entities;

namespace SupportTicketPlatform.Worker.Application.Interfaces;

public interface ITicketAnalysisService
{
    Task<string> GenerateAnalysisAsync(Ticket ticket, IReadOnlyList<Ticket> resolvedTickets, CancellationToken cancellationToken);
}
