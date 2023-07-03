using System.IO;
using Common.IO.Serialize;

namespace Common.IO.Streams
{
    /// <summary>
    /// BinaryWriter extension
    /// </summary>
    public class BinaryWriterEx : BinaryWriter
    {

        public BinaryWriterEx(Stream output) : base(output)
        {
        }
        
        public void WriteInt(int v)
        {
            Write(v);
        }

        public void WriteString(string v)
        {
            Write(v);
        }

        /// <summary>
        /// write serializable to file
        /// </summary>
        /// <param name="sd"></param>
        /// <param name="filePath"></param>
        public static void Write(IBinarySerializable sd, string filePath)
        {
            using (var stream = File.OpenWrite(filePath))
            {
                Write(sd, stream);
            }
        }

        /// <summary>
        /// write serializable to stream 
        /// </summary>
        /// <param name="sd"></param>
        /// <param name="stream"></param>
        public static void Write(IBinarySerializable sd, Stream stream)
        {
            var writer = new BinaryWriterEx(stream);
            sd.Write(writer);
        }
    }
}