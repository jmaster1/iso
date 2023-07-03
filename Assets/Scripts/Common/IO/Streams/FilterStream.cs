using System.IO;

namespace Common.IO.Streams
{
    /// <summary>
    /// FilterStream that delegates i/o to target
    /// </summary>
    public class FilterStream : Stream
    {
        private Stream target;

        public FilterStream(Stream stream)
        {
            target = stream;
        }

        public override void Flush()
        {
            target.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return target.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return target.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            target.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            target.Write(buffer, offset, count);
        }

        public override bool CanRead => target.CanRead;

        public override bool CanSeek => target.CanSeek;

        public override bool CanTimeout => target.CanTimeout;

        public override bool CanWrite => target.CanWrite;

        public override long Length => target.Length;

        public override long Position
        {
            get => target.Position;
            set => target.Position = value;
        }

        public override int ReadTimeout
        {
            get => target.ReadTimeout;
            set => target.ReadTimeout = value;
        }

        public override int WriteTimeout
        {
            get => target.WriteTimeout;
            set => target.WriteTimeout = value;
        }
    }
}
