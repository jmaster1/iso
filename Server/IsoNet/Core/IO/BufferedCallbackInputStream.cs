namespace IsoNet.Core.IO;

using System;
using System.IO;

public class BufferedCallbackInputStream : Stream
{
    private readonly Func<byte[], (int, bool)> _fillAction;
    private readonly byte[] _buffer;
    private int _bufferOffset;
    private int _bufferCount;
    private bool _endOfStream;
    private bool _lastFill;
    
    public BufferedCallbackInputStream(byte[] buffer, Func<byte[], (int, bool)> fillAction)
    {
        _fillAction = fillAction;
        _buffer = buffer;
        _bufferOffset = 0;
        _bufferCount = 0;
        _endOfStream = false;
    }
    
    public BufferedCallbackInputStream(int bufferSize, Func<byte[], (int, bool)> fillAction) : 
        this(new byte[bufferSize], fillAction)
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (_endOfStream) return 0;

        if (_bufferOffset >= _bufferCount)
        {
            if (_lastFill)
            {
                _endOfStream = true;
                return 0;
            }
            
            (_bufferCount, _lastFill) = _fillAction(_buffer);
            _bufferOffset = 0;

            if (_bufferCount == 0)
            {
                _endOfStream = true;
                return 0;
            }
        }

        var bytesToRead = Math.Min(count, _bufferCount - _bufferOffset);
        Array.Copy(_buffer, _bufferOffset, buffer, offset, bytesToRead);
        _bufferOffset += bytesToRead;

        return bytesToRead;
    }

    public void Filled(int count, bool lastFill)
    {
        _bufferCount = count;
        _lastFill = lastFill;
        _bufferOffset = 0;
        _endOfStream = false;
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }
    public override void Flush() => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
}
