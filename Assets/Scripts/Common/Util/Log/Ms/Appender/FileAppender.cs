using System;
using System.Collections.Concurrent;
using System.IO;

namespace IsoNet.Core.Log.Appender
{
    public class FileAppender : IAppender
    {
        private static readonly ConcurrentDictionary<string, FileAppender> Appenders = new();

        private readonly string _filePath;
        private readonly object _fileLock;

        public FileAppender(string filePath, object fileLock)
        {
            _filePath = filePath;
            _fileLock = fileLock;
        }

        public static IAppender? AnnounceAppender;

        public void Append(string text)
        {
            lock (_fileLock)
            {
                File.AppendAllText(_filePath, text);
            }
        }

        public static IAppender Create(object instance, string suffix = ".log", 
            Action<IAppender>? initializer = null)
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
}
