using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Common.IO.FileSystem
{
    /// <summary>
    /// AbstractFileSystem extension that works with ZipArchive
    /// </summary>
    public class ZipFileSystem : AbstractFileSystem
    {
        /// <summary>
        /// archive
        /// </summary>
        public readonly ZipArchive Archive;

        /// <summary>
        /// memory stream for crated archive
        /// </summary>
        private readonly MemoryStream memoryStream;

        public ZipFileSystem(ZipArchive archive)
        {
            Archive = archive;
        }
        
        public ZipFileSystem()
        {
            memoryStream = new MemoryStream();
            Archive = new ZipArchive(memoryStream, ZipArchiveMode.Create);
        }

        public override Stream GetStream(string name, bool write)
        {
            var entry = write ? Archive.CreateEntry(name) : Archive.GetEntry(name);
            return entry?.Open();
        }

        public override long GetLength(string name)
        {
            var entry = Archive.GetEntry(name);
            return entry?.Length ?? LengthUnknown;
        }

        public override bool Exists(string name)
        {
            return Archive.GetEntry(name) != null;
        }

        public override List<string> List(Func<string, bool> fileFilter = null)
        {
            var filter = fileFilter ?? (s => true);
            var n = Archive.Entries.Count;
            var ret = new List<string>(n);
            ret.AddRange(from e in Archive.Entries where filter.Invoke(e.Name) select e.Name);
            return ret;
        }

        /// <summary>
        /// retrieve bytes of zip archive, should be called only when created with empty ctor
        /// </summary>
        /// <returns></returns>
        public MemoryStream GetZipStream()
        {
            Archive.Dispose();
            return new MemoryStream(memoryStream.ToArray());
        }
    }
}