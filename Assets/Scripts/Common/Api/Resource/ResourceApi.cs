using System;
using System.IO;
using Common.IO.Serialize;
using Common.IO.Streams;
using Common.Util;

namespace Common.Api.Resource
{
    /// <summary>
    /// resource access api
    /// </summary>
    public class ResourceApi : AbstractApi
    {
        public Func<string, Stream> StreamFactory;
        
        /// <summary>
        /// default extension to be used for naming binary files in unity
        /// </summary>
        public const string FileExtension = ".bytes";

        /// <summary>
        /// json serializer
        /// </summary>
        public JsonObjectSerializer JsonSerializer = new NewtonsoftJsonObjectSerializer();

        /// <summary>
        /// retrieve resource stream from path
        /// </summary>
        public Stream Read(string path)
        {
            LangHelper.Validate(StreamFactory != null);
            return StreamFactory(path);
        }
        
        /// <summary>
        /// load named resource as object
        /// </summary>
        public object LoadObject(string path, Type type)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("name={0}, type={1}", path, type);
            } 
            using (var stream = Read(path))
            {
                return JsonSerializer.Read(stream, type);
            }
        }
        
        /// <summary>
        /// retrieve resource binary reader from path
        /// </summary>
        public BinaryReaderEx ReadResource(string path)
        {
            var stream = Read(path);
            return new BinaryReaderEx(stream);
        }
    }
}
