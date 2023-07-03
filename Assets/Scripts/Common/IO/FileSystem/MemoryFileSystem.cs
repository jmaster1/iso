using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.IO.Streams;
using Common.Lang;

namespace Common.IO.FileSystem
{
    /// <summary>
    /// AbstractFileSystem extension that works with Memory.
    /// we need to use MemoryStreamEx that doesnt close on dispose
    /// </summary>
    public class MemoryFileSystem : AbstractFileSystem
    {
        /// <summary>
        /// contains live data streams
        /// </summary>
        private readonly Map<string, MemoryStreamEx> cache = new Map<string, MemoryStreamEx>();
        
        /// <summary>
        /// here we put streams on Clear() to reuse them 
        /// </summary>
        private readonly Map<string, MemoryStreamEx> removed = new Map<string, MemoryStreamEx>();

        public bool IsEmpty => cache.Count == 0;

        public override Stream GetStream(string name, bool write)
        {
            var ms = cache.Find(name);
            if (write && ms == null)
            {
                ms = removed.Find(name);
                if (ms == null)
                {
                    cache[name] = ms = new MemoryStreamEx();
                    ms.PreventDispose = true;
                }
                cache[name] = ms;
            }

            if (ms != null)
            {
                ms.SetLength(ms.Position);
                ms.Position = 0;
            }
            return ms;
        }

        public override long GetLength(string name)
        {
            var ms = cache.Find(name);
            return ms?.Length ?? LengthUnknown;
        }

        public override bool Exists(string name)
        {
            return cache.ContainsKey(name);
        }

        public override List<string> List(Func<string, bool> fileFilter = null)
        {
            var filter = fileFilter ?? (s => true);
            return cache.Keys.Where(filter).ToList();
        }

        public override void Clear()
        {
            foreach (var kv in cache)
            {
                removed[kv.Key] = kv.Value;
            }
            cache.Clear();
        }
    }
}
