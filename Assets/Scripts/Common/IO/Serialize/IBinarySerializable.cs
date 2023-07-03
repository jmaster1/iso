using Common.IO.Streams;

namespace Common.IO.Serialize
{
    /// <summary>
    /// binary [de]serializable object api definition 
    /// </summary>
    public interface IBinarySerializable
    {
        /// <summary>
        /// write state to writer
        /// </summary>
        void Write(BinaryWriterEx w);
        
        /// <summary>
        /// read state from reader
        /// </summary>
        void Read(BinaryReaderEx r);
    }
}
