using System;
using System.IO;
using System.Text;

namespace Common.IO.Codec
{
    public abstract class AbstractTextCodec : ICodec
    {
        protected abstract void Write(object? item, TextWriter writer);

        protected abstract object? Read(TextReader reader, Type type);
    
        public void Write(object? item, Stream target)
        {
            using var writer = new StreamWriter(target, Encoding.UTF8, 1024, leaveOpen: true);
            Write(item, writer);
            writer.Flush();
        }

        public object? Read(Stream source, Type type)
        {
            var reader = new StreamReader(source, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
            return Read(reader, type);
        }

        public T? Read<T>(Stream source)
        {
            return (T?) Read(source, typeof(T));
        }
    }
}
