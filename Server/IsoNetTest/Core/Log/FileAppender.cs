using System.Collections.Concurrent;

namespace IsoNetTest.Core.Log;

public static class FileAppender
{
    private static readonly object FileLock = new();
    
    private static readonly ConcurrentDictionary<string, object> FileLocks = new();
    
    public static void Append(string filePath, string text)
    {
        var fileLock = FileLocks.TryGetValue(filePath, out var existing) 
            ? existing 
            : FileLocks[filePath] = new object();

        lock (fileLock)
        {
            File.AppendAllText(filePath, text);
        }
    }
}