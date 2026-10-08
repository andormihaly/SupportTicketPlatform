using System.Diagnostics;

namespace SupportTicketPlatform.Infrastructure.Telemetry;

public sealed class OpenTelemetryApplicationTelemetry : IApplicationTelemetry
{
    private static readonly ActivitySource ActivitySource = new("SupportTicketPlatform.Application");

    public IDisposable? StartActivity(string name)
    {
        return ActivitySource.StartActivity(name);
    }
    public void RecordException(Exception exception)
    {
        var activity = Activity.Current;

        if (activity is null)
            return;

        activity.SetStatus(ActivityStatusCode.Error, exception.Message);
        activity.AddException(exception);
    }
    public IDisposable? StartActivityWithTags(string name, IReadOnlyDictionary<string, object?> tags)
    {
        var activity = ActivitySource.StartActivity(name);

        if (activity is not null)
        {
            foreach (var tag in tags)
                activity.SetTag(tag.Key, tag.Value);
        }

        return activity;
    }
}
