using Microsoft.Extensions.Logging;

namespace IsoNet.Core.IO.Codec;

public static class CodecExtensions
{
    public static LoggingCodec WrapLogging(this ICodec codec, ILogger? logger)
    {
        return new LoggingCodec(codec, logger);
    }
}