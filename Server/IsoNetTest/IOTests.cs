using IsoNet.Core.IO;
using Microsoft.Extensions.Logging;

namespace IsoNetTest;

public class IoTests : AbstractTests
{
    [Test]
    public void TestBufferedCallbackStream()
    {
        var logger = CreateLogger("Output");
        var ms = new MemoryStream();
        const int len = 100;
        var eofResult = false;
        logger.LogInformation("Start writing {len} bytes...", len);
        var stream = new BufferedCallbackOutputStream(8, (bytes, count, eof) =>
        {
            ms.Write(bytes, 0, count);
            eofResult = eof;
            logger.LogInformation("Wrote {count} bytes, eof={eof}", count, eof);
        });
        logger.LogInformation("End writing");
        
        
        for (var i = 0; i < len; i++)
        {
            stream.Write([(byte)i]);
        }
        stream.Flush();
        
        Assert.That(eofResult, Is.EqualTo(true));
        
        var buf = ms.GetBuffer();
        for (var i = 0; i < len; i++)
        {
            Assert.That(buf[i], Is.EqualTo(i));
        }
    }
    
    [Test]
    public void TestBufferedCallbackInputStream()
    {
        var logger = CreateLogger("Input");
        const int len = 100;
        var pos = 0;

        (int, bool) FillBuffer(byte[] buffer)
        {
            if (pos >= len) return (0, true);
            var written = 0;
            while (pos < len && written < buffer.Length)
            {
                buffer[written++] = (byte)pos++;
            }
            logger.LogInformation("Wrote {written} bytes", written);
            return (written, false);
        }

        using var stream = new BufferedCallbackInputStream(8, FillBuffer);

        logger.LogInformation("Start reading {len} bytes...", len);
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        logger.LogInformation("End reading");
        var data = ms.ToArray();
        Assert.That(data, Has.Length.EqualTo(len));
        for (var i = 0; i < len; i++)
        {
            Assert.That(data[i], Is.EqualTo(i));
        }
    }
}
