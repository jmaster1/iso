using System.IO;
using Common.IO.FileSystem;
using Common.IO.Streams;
using Common.Util;
using UnityEngine;

namespace Common.Unity.IO.FileSystem
{
    /// <summary>
    /// FileSystem based on PlayerPrefs that uses serialized strings
    /// </summary>
    public class PlayerPrefsFileSystem : AbstractFileSystem
    {
        public override Stream GetStream(string name, bool write)
        {
            Stream ret = null;
            if (write)
            {
                var ms = new MemoryStreamEx();
                ms.DisposeAction = () =>
                {
                    var text = StringHelper.GetString(ms);
                    PlayerPrefs.SetString(name, text);
                };   
            }
            else
            {
                var data = PlayerPrefs.GetString(name);
                ret = StringHelper.GetStream(data);
            }
            return ret;
        }

        public override long GetLength(string name)
        {
            return LengthUnknown;
        }
    }
}