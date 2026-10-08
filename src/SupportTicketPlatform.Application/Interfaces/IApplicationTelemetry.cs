public interface IApplicationTelemetry
{
    IDisposable? StartActivity(string name);
    IDisposable? StartActivityWithTags(string name, IReadOnlyDictionary<string, object?> tags);
    void RecordException(Exception exception);
}
