namespace IsoNet.Core.IO;

using System;
using System.IO;

public class BufferedCallbackOutputStream : Stream
{
    private readonly int _bufferSize;
    private readonly Action<byte[], int, bool> _bufferFilledCallback;
    private readonly byte[] _buffer;
    private int _bufferCount;

    public BufferedCallbackOutputStream(byte[] buffer, Action<byte[], int, bool> bufferFilledCallback)
    {
        _buffer = buffer;
        _bufferSize = _buffer.Length;
        _bufferFilledCallback = bufferFilledCallback ?? throw new ArgumentNullException(nameof(bufferFilledCallback));
        _bufferCount = 0;
    }
    

    public BufferedCallbackOutputStream(int bufferSize, Action<byte[], int, bool> bufferFilledCallback) :
        this(new byte[bufferSize], bufferFilledCallback)
    {
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        var bytesProcessed = 0;

        while (bytesProcessed < count)
        {
            var bytesToCopy = Math.Min(_bufferSize - _bufferCount, count - bytesProcessed);
            Array.Copy(buffer, offset + bytesProcessed, _buffer, _bufferCount, bytesToCopy);
            _bufferCount += bytesToCopy;
            bytesProcessed += bytesToCopy;

            if (_bufferCount != _bufferSize) continue;
            _bufferFilledCallback(_buffer, _bufferCount, false);
            _bufferCount = 0;
        }
    }

    public override void Flush()
    {
        if (_bufferCount <= 0) return;
        _bufferFilledCallback(_buffer, _bufferCount, true);
        _bufferCount = 0;
    }

    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => throw new NotSupportedException();
    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }
    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();

    public void Reset()
    {
        _bufferCount = 0;
    }
}
