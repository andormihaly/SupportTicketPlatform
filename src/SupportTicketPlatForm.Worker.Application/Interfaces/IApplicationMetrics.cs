namespace SupportTicketPlatform.Worker.Application.Interfaces;

public interface IApplicationMetrics
{
    void TicketCreated();
    void TicketAnalysisCompleted(double durationSeconds);
}
