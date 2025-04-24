using Microsoft.Extensions.Logging;

namespace IsoNetTest.Core;

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

internal class TestContextLogger(string category) : ILogger
{
    private static readonly object _fileLock = new();
    private static readonly string LogFilePath = Path.Combine(Directory.GetCurrentDirectory(), "test-log.txt");

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var msg = $"[{DateTime.Now:HH:mm:ss.fff}] [{logLevel}] [{category}] [{Thread.CurrentThread.Name} @ {Environment.CurrentManagedThreadId} ] {formatter(state, exception)}";
        TestContext.Progress.WriteLine(msg);
        TestContext.Progress.Flush();
        
        lock (_fileLock)
        {
            File.AppendAllText(LogFilePath, msg + Environment.NewLine);
        }
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
}
