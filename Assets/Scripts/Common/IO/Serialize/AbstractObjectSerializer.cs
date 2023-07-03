using System;
using System.IO;
using Common.IO.FileSystem;

namespace Common.IO.Serialize
{
    /// <summary>
    /// base class that is capable of [de]serializing objects to/from stream
    /// </summary>
    public abstract class AbstractObjectSerializer
    {
        /// <summary>
        /// format name retrieval
        /// </summary>
        public abstract string GetFormat();
        
        /// <summary>
        /// stream name resolver for object type
        /// </summary>
        /// <returns>{type.name}.{format}</returns>
        public string GetStreamName(Type type)
        {
            return type.Name + "." + GetFormat();
        }
        
        /// <summary>
        /// write object to stream
        /// </summary>
        public abstract void Write(object obj, Stream output);
        
        /// <summary>
        /// write entity to filesystem using type name for stream resolution
        /// </summary>
        public void Write(object obj, AbstractFileSystem fs)
        {
            var type = obj.GetType();
            var name = GetStreamName(type);
            using var output = fs.Write(name);
            Write(obj, output);
        }

        public T Read<T>(Stream input)
        {
            var type = typeof(T);
            return (T) Read(input, type);
        }
        
        /// <summary>
        /// read entity from filesystem using type name for stream resolution
        /// </summary>
        /// <returns>null if no such stream</returns>
        public T Read<T>(AbstractFileSystem fs)
        {
            var type = typeof(T);
            var name = GetStreamName(type);
            using (var input = fs.Read(name))
            {
                if (input != null) return (T) Read(input, type);
            }
            return default;
        }

        /// <summary>
        /// read object from stream
        /// </summary>
        public abstract object Read(Stream input, Type type);
    }
}
