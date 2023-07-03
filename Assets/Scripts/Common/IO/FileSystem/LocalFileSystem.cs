using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Common.IO.FileSystem
{
    /// <summary>
    /// AbstractFileSystem extension that works with local files
    /// </summary>
    public class LocalFileSystem : AbstractFileSystem
    {
        /// <summary>
        /// root folder path
        /// </summary>
        public readonly string Root;

        public LocalFileSystem(string root)
        {
            Root = Path.GetFullPath(root);
        }

        /// <summary>
        /// retrieve full path to named file
        /// </summary>
        string GetLocalPath(string name)
        {
            return Path.Combine(Root, name);
        }
        
        /// <summary>
        /// convert local path to virtual
        /// </summary>
        string GetVirtualPath(string localPath)
        {
            var ret = localPath.Substring(Root.Length + 1);
            if (Path.DirectorySeparatorChar != DirectorySeparator)
            {
                ret = ret.Replace(Path.DirectorySeparatorChar, DirectorySeparator);
            }
            return ret;
        }

        public override Stream GetStream(string name, bool write)
        {
            var path = GetLocalPath(name);
            var exists = File.Exists(path);
            if (!write && !exists) return null;
            if (write && !exists)
            {
                var dirPath = Path.GetDirectoryName(path);
                Directory.CreateDirectory(dirPath);
            }
            return write ? File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read) : 
                File.OpenRead(path);
        }

        public override long GetLength(string name)
        {
            var path = GetLocalPath(name);
            if (!File.Exists(path)) return LengthUnknown;
            var info = new FileInfo(path);
            return info.Length;
        }

        public override void Move(string oldName, string newName)
        {
            var oldPath = GetLocalPath(oldName);
            Validate(File.Exists(oldPath), "File not exists: {0}", oldPath);
            var newPath = GetLocalPath(newName);
            if (File.Exists(newPath))
            {
                File.Delete(newPath);
            }
            File.Move(oldPath, newPath);
        }
        
        public override void Delete(string name)
        {
            var path = GetLocalPath(name);
            File.Delete(path);
        }

        public override bool Exists(string name)
        {
            var path = GetLocalPath(name);
            return File.Exists(path);
        }

        public override List<string> List(Func<string, bool> fileFilter = null)
        {
            Func<string, bool> filter;
            if (fileFilter == null)
                filter = s => true;
            else
                filter = s => fileFilter(GetVirtualPath(s));
            return Directory
                .EnumerateFiles(Root, "*", SearchOption.AllDirectories)
                .Where(filter)
                .Select(s => GetVirtualPath(s))
                .ToList();
        }
    }
}
