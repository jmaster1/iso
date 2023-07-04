using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace Common.Unity.Util.EventTracker
{
    public class ZipRemoteStream : Stream
    {
        private readonly FileStream _inputFile;
        
        private readonly GZipStream _gzip;
        private readonly CryptoStream _base64;
        
        private readonly byte[] _inputBuffer;
        private bool _inputActive = true;

        private int _outputBufferIndex;
        private byte[] _outputBuffer;
        
        private readonly byte[] _header;
        private readonly byte[] _footer;
        private bool _isHeaderWrote;
        private bool _isFooterWrote;

        private bool _isDisposed;

        public ZipRemoteStream(FileStream input, string header, string footer, int bufferSize = 40000)
        {
            _inputBuffer = new byte[bufferSize];
            _outputBuffer = new byte[bufferSize];
            _header = Encoding.ASCII.GetBytes(header);
            _footer = Encoding.ASCII.GetBytes(footer);
            
            _inputFile = input;
            _base64 = new CryptoStream(this, new ToBase64Transform(), CryptoStreamMode.Write);
            _gzip = new GZipStream(_base64, CompressionMode.Compress, true);
        }

        public override void Flush()
        {
        }
        
        public override int Read(byte[] buffer, int offset, int count)
        {
            int off = offset;
            int cnt = count;
            int totalRead = 0;
            
            while (true)
            {
                if (_outputBufferIndex != 0)
                {
                    if (_outputBufferIndex <= cnt)
                    {
                        Buffer.BlockCopy(_outputBuffer, 0, buffer, off, _outputBufferIndex);
                        cnt -= _outputBufferIndex;
                        off += _outputBufferIndex;
                        totalRead += _outputBufferIndex;
                        _outputBufferIndex = 0;
                    }
                    else
                    {
                        Buffer.BlockCopy(_outputBuffer, 0, buffer, off, cnt);
                        Buffer.BlockCopy(_outputBuffer, cnt, _outputBuffer, 0, _outputBufferIndex - cnt);
                        _outputBufferIndex -= cnt;
                        totalRead += cnt;
                        break;
                    }
                }
                
                if (_inputActive)
                {
                    int rr = _inputFile.Read(_inputBuffer, 0, _inputBuffer.Length);
                    if (rr > 0)
                    {
                        _gzip.Write(_inputBuffer, 0, rr);
                    }
                    else
                    {
                        _inputActive = false;
                        _gzip.Close();
                        _base64.Close();
                    }
                }
                
                if (_outputBufferIndex != 0 || _inputActive) continue;
                                
                if (!_isFooterWrote)
                {
                    _isFooterWrote = true;
                    _outputBufferIndex = _footer.Length;
                    Buffer.BlockCopy(_footer, 0, _outputBuffer, 0, _outputBufferIndex);
                    continue;
                }
                
                break;
            }

            return totalRead;
        }

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _gzip.Dispose();
            _base64.Dispose();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!_isHeaderWrote)
            {
                _isHeaderWrote = true;
                _outputBufferIndex = _header.Length;
                Buffer.BlockCopy(_header, 0, _outputBuffer, 0, _outputBufferIndex);
            }

            if (count > _outputBuffer.Length - _outputBufferIndex)
            {
                byte[] tmp = new byte[(_outputBufferIndex + count) * 3 / 2];
                Buffer.BlockCopy(_outputBuffer, 0, tmp, 0, _outputBufferIndex);
                _outputBuffer = tmp;
            }
            Buffer.BlockCopy(buffer, 0, _outputBuffer, _outputBufferIndex, count);
            _outputBufferIndex += count;
        }
        
        public override void SetLength(long value)
        {
        }
        
        public override long Seek(long offset, SeekOrigin origin)
        {
            return 0;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public override long Length => throw new NotSupportedException("NotSupported");
        public override long Position { get; set; }
    }
}