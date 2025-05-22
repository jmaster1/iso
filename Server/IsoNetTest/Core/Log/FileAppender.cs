using System.Collections.Concurrent;

namespace IsoNetTest.Core.Log;

public class FileAppender(string filePath, object fileLock) : IAppender
{
    private static readonly ConcurrentDictionary<string, FileAppender> Appenders = new();
    
    public static IAppender? AnnounceAppender;

    public void Append(string text)
    {
        lock (fileLock)
        {
            File.AppendAllText(filePath, text);
        }
    }

    public static IAppender Create(object instance, string suffix = ".log", Action<IAppender>? initializer = null)
    {
        var fileName = instance.GetType().Name + suffix;
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        var appender = Appenders.TryGetValue(filePath, out var existing) 
            ? existing 
            : Appenders[filePath] = new FileAppender(filePath, new object());
        if (existing is not null) return appender;
        AnnounceAppender?.Append("Writing to file: " + filePath);
        File.WriteAllText(filePath, string.Empty);
        initializer?.Invoke(appender);
        return appender;
    }
}
