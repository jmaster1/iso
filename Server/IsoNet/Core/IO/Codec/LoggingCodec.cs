using System.Text;
using Microsoft.Extensions.Logging;

namespace IsoNet.Core.IO.Codec;

public class LoggingCodec : LogAware, ICodec
{
    public const string EventNameWrite = "LoggingCodec.Write";
    public const string EventNameWriteError = "LoggingCodec.WriteError";
    public const string EventNameRead = "LoggingCodec.Read";
    public const string EventNameReadError = "LoggingCodec.ReadError";

    public static readonly EventId EventIdWrite = new(1, EventNameWrite);
    public static readonly EventId EventIdRead = new(2, EventNameRead);
    public static readonly EventId EventIdReadError = new(3, EventNameReadError);
    public static readonly EventId EventIdWriteError = new(4, EventNameWriteError);
    
    private readonly ICodec _codec;
    
    public LoggingCodec(ICodec codec, ILogger? logger = null)
    {
        _codec = codec;
        Logger = logger;
    }
    
    public void Write(object? item, Stream target)
    {
        using var ms = new MemoryStream();
        try
        {
            _codec.Write(item, ms);    
        }
        catch (Exception ex)
        {
            Logger?.LogError(EventIdWriteError, "WriteError: ex={ex}", ex);
            throw;
        }
        
        var bytes = ms.ToArray();
        var str = Encoding.UTF8.GetString(bytes);
        Logger?.LogInformation(EventIdWrite, "Write: {str}", str);
        target.Write(bytes, 0, bytes.Length);
        target.Flush();
    }

    public object? Read(Stream source, Type type)
    {
        using var ms = new MemoryStream();
        source.CopyTo(ms);
        ms.Position = 0;
        var bytes = ms.ToArray();
        var str = Encoding.UTF8.GetString(bytes);
        Logger?.LogInformation(EventIdRead, "Read: {type}={str}", type, str);
        try
        {
            return _codec.Read(ms, type);
        }
        catch (Exception ex)
        {
            Logger?.LogError(EventIdReadError, "ReadError: {type}={str}, ex={ex}", type, str, ex);
            throw;
        }
    }
}
