namespace IsoNetTest;

using Microsoft.Extensions.Logging;

public class TestContextLoggerProvider : ILoggerProvider
{
    public void Dispose()
    {
    }

    public ILogger CreateLogger(string category)
    {
        return new TestContextLogger(category);
    }
}

internal class TestContextLogger : ILogger
{
    private readonly string category;

    public TestContextLogger(string category)
    {
        this.category = category;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var msg = $"[{DateTime.Now:HH:mm:ss.fff}] [{logLevel}] [{category}] [{Thread.CurrentThread.Name} @ {Environment.CurrentManagedThreadId} ] {formatter(state, exception)}";
        TestContext.Progress.WriteLine(msg);
        TestContext.Progress.Flush();
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
}
