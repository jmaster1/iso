using System.Text;
using Microsoft.Extensions.Logging;

namespace IsoNet.Core.IO.Codec;

public class LoggingCodec2 : LogAware, ICodec2
{
    private readonly ICodec2 _codec;
    
    public LoggingCodec2(ICodec2 codec, ILogger? logger = null)
    {
        _codec = codec;
        Logger = logger;
    }
    
    public void Write<T>(T item, Stream target)
    {
        using var ms = new MemoryStream();
        _codec.Write(item, ms);
        var bytes = ms.ToArray();
        var str = Encoding.UTF8.GetString(bytes);
        Logger?.LogInformation("Write: {str}", str);
        target.Write(bytes, 0, bytes.Length);
        target.Flush();
    }

    public T Read<T>(Stream source)
    {
        using var ms = new MemoryStream();
        source.CopyTo(ms);
        ms.Position = 0;
        var bytes = ms.ToArray();
        var str = Encoding.UTF8.GetString(bytes);
        Logger?.LogInformation("Read: {str}", str);
        return _codec.Read<T>(ms);
    }
}
