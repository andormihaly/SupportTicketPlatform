using SupportTicketPlatform.Worker.Application.Interfaces;
using System.Diagnostics.Metrics;

namespace SupportTicketPlatform.Worker.Infrastructure.Telemetry;

public sealed class OpenTelemetryApplicationMetrics : IApplicationMetrics
{
    public const string MeterName = "SupportTicketPlatform.Worker.Application";

    private static readonly Meter Meter = new(MeterName);

    private static readonly Counter<long> TicketsCreated = Meter.CreateCounter<long>("support_tickets.created");

    private static readonly Counter<long> TicketAnalysesCompleted = Meter.CreateCounter<long>("support_tickets.analysis.completed");

    private static readonly Histogram<double> TicketAnalysisDuration = Meter.CreateHistogram<double>("support_tickets.analysis.duration", "s");

    public void TicketCreated()
    {
        TicketsCreated.Add(1);
    }

    public void TicketAnalysisCompleted(double durationSeconds)
    {
        TicketAnalysesCompleted.Add(1);
        TicketAnalysisDuration.Record(durationSeconds);
    }
}
