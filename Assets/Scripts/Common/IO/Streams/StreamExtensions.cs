using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Common.IO.Streams
{
    public static class StreamExtensions
    {
        
        /// <summary>
        /// ensure encoding non null (use UTF-8 by default)
        /// </summary>
        public static Encoding CheckEncoding(Encoding encoding)
        {
            return encoding ?? Encoding.UTF8;
        }
        
        /// <summary>
        /// read all bytes from stream
        /// </summary>
        public static byte[] ReadBytes(this Stream stream)
        {
            using(var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        
        /// <summary>
        /// write all bytes to stream
        /// </summary>
        public static void WriteBytes(this Stream stream, byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return;
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// read string from stream
        /// </summary>
        public static string ReadString(this Stream stream, 
            Encoding encoding = null,
            bool detectEncoding = false,
            int bufferSize = 4096, 
            bool leaveOpen = true)
        {
            encoding = CheckEncoding(encoding);
            using (var streamReader = new StreamReader(
                stream, encoding, detectEncoding, bufferSize, leaveOpen))
                return streamReader.ReadToEnd();
        }
        /// <summary>
        /// write string to stream
        /// </summary>
        public static void WriteString(this Stream stream, string str, Encoding encoding = null)
        {
            encoding = CheckEncoding(encoding);
            var bytes = encoding.GetBytes(str);
            stream.WriteBytes(bytes);
        }
        
        public static GZipStream GZip(this Stream stream, CompressionLevel level = CompressionLevel.Optimal) => 
            new(stream, level);
        
        public static GZipStream GUnZip(this Stream stream) => 
            new(stream,  CompressionMode.Decompress);

        public static BinaryWriter DataWriter(this Stream stream) => new(stream);
        public static BinaryReader DataReader(this Stream stream) => new(stream);
        
        public static string ToBase64String(this MemoryStream stream) =>
            Convert.ToBase64String(stream.ToArray());
        
        public static byte[] FromBase64String(this string str) =>
            Convert.FromBase64String(str);
        
        public static MemoryStream FromBase64StringStream(this string str) => 
            new(str.FromBase64String());
    }
}