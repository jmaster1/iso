using Microsoft.Extensions.Logging;

namespace IsoNetTest.Core.Log;

public class DefaultLogger : AbstractLogger
{
    public override void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, 
        Func<TState, Exception?, string> formatter)
    {
        var msg = $"" +
                  $"[{DateTime.Now:HH:mm:ss.fff}] " +
                  $"[{logLevel}] [{Category}] " +
                  $"[{Thread.CurrentThread.Name} @ {Environment.CurrentManagedThreadId} ] " +
                  $"{formatter(state, exception)}";
        Append(msg);
    }
}
