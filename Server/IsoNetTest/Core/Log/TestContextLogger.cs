using Microsoft.Extensions.Logging;

namespace IsoNetTest.Core.Log;

public class TestContextLogger(string category) : AbstractLogger
{
    public static readonly ILoggerProvider Provider = new LoggerProvider(
        category => new TestContextLogger(category));
    
    private static readonly string LogFilePath = Path.Combine(Directory.GetCurrentDirectory(), "test-log.txt");

    public override void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, 
        Func<TState, Exception?, string> formatter)
    {
        var msg = $"" +
                  $"[{DateTime.Now:HH:mm:ss.fff}] " +
                  $"[{logLevel}] [{category}] " +
                  $"[{Thread.CurrentThread.Name} @ {Environment.CurrentManagedThreadId} ] " +
                  $"{formatter(state, exception)}";
        TestContext.Progress.WriteLine(msg);
        TestContext.Progress.Flush();

        FileAppender.Append(LogFilePath, msg + Environment.NewLine);
    }
}
