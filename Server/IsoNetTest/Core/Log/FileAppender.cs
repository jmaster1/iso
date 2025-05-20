using System.Collections.Concurrent;

namespace IsoNetTest.Core.Log;

public static class FileAppender
{
    private static readonly ConcurrentDictionary<string, object> FileLocks = new();
    
    public static void Append(string filePath, string text)
    {
        var fileLock = FileLocks.TryGetValue(filePath, out var existing) 
            ? existing 
            : FileLocks[filePath] = CreateFileLock(filePath);

        lock (fileLock)
        {
            File.AppendAllText(filePath, text);
        }
    }

    private static object CreateFileLock(string filePath)
    {
        
        return new object();
    }

    public static string LogFilePath(string fileName, Func<string>? contentProvider = null)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        File.WriteAllText(filePath, contentProvider == null ? 
            string.Empty : contentProvider());
        return filePath;
    }
}
