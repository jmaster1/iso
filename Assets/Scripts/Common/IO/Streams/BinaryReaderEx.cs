using System.IO;

namespace Common.IO.Streams
{
    /// <summary>
    /// BinaryReader extension
    /// </summary>
    public class BinaryReaderEx : BinaryReader
    {
        public BinaryReaderEx(Stream input) : base(input)
        {
        }

        public int ReadInt()
        {
            return ReadInt32();
        }
    }
}