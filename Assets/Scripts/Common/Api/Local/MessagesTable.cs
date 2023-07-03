using Common.IO.Serialize;
using Common.IO.Streams;
using Common.Lang;
using Common.Lang.Entity;

namespace Common.Api.Local
{
    /// <summary>
    /// localization messages container, id matches language code 
    /// </summary>
    public class MessagesTable : AbstractEntityIdString, IBinarySerializable
    {
        /// <summary>
        /// mapping key hash to value 
        /// </summary>
        public Map<int, string> Map;

        public void Write(BinaryWriterEx w)
        {
            w.WriteString(Id);
            w.Write(Map.Count);
            foreach (var e in Map)
            {
                w.WriteInt(e.Key);
                w.WriteString(e.Value);
            }
        }
        
        public void Read(BinaryReaderEx r)
        {
            Id = r.ReadString();
            int size = r.ReadInt();
            Map = new Map<int, string>(size);
            for (int i = 0; i < size; i++)
            {
                int key = r.ReadInt();
                string value = r.ReadString();
                Map.Add(key, value);
            }
        }
    }
}
