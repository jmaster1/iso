using Microsoft.Extensions.Logging;

namespace IsoNetTest.Core.Log;

public abstract class AbstractLogger : ILogger
{
    public abstract void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter);
    
    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
}

public class LoggerProvider(Func<string, ILogger> factory) : ILoggerProvider
{
    public void Dispose()
    {
    }

    public ILogger CreateLogger(string category)
    {
        return factory(category);
    }
}