using Microsoft.Extensions.Logging;

namespace IsoNetTest.Core.Log;

public class HtmlLogger : ILogger
{
    public static readonly ILoggerProvider Provider = new LoggerProvider(
        category => new HtmlLogger(category));
    
    private readonly string _categoryName;

    public HtmlLogger(string categoryName)
    {
        _categoryName = categoryName;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception, string> formatter)
    {
        Console.WriteLine($"[{logLevel}] {_categoryName}: {formatter(state, exception)}");
    }
}