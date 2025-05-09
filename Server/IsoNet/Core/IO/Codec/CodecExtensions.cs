using Microsoft.Extensions.Logging;

namespace IsoNet.Core.IO.Codec;

public static class CodecExtensions
{
    public static LoggingCodec<T> WrapLogging<T>(this ICodec<T> codec, ILogger? logger)
    {
        return new LoggingCodec<T>(codec, logger);
    }
    
    public static LoggingCodec<T> WrapLogging<T>(this ICodec<T> codec, ILogger? logger)
    {
        return new LoggingCodec<T>(codec, logger);
    }
}