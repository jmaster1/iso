using System;
using System.Collections.Generic;
using System.IO;
using Common.IO.Streams;
using Common.Lang.Entity;

namespace Common.IO.FileSystem
{
    /// <summary>
    /// base class that provides i/o access to named files
    /// </summary>
    public abstract class AbstractFileSystem : GenericBean
    {
        /// <summary>
        /// directory separator in file path
        /// </summary>
        public const char DirectorySeparator = '/';
        
        /// <summary>
        /// unknown file length value
        /// </summary>
        public const long LengthUnknown = -1;
        
        /// <summary>
        /// subclasses should implement stream retrieval for i/o
        /// </summary>
        /// <param name="name">name of file</param>
        /// <param name="write">true if requested stream for output</param>
        /// <returns>null if stream for read not exists</returns>
        public abstract Stream GetStream(string name, bool write);
        
        public abstract long GetLength(string name);

        public Stream Read(string name) => GetStream(name, false);
        
        public TextReader TextReader(string name)
        {
            var stream = Read(name);
            return stream == null ? null : new StreamReader(stream);
        }

        /// <summary>
        /// read all bytes from named file
        /// </summary>
        public byte[] ReadBytes(string name)
        {
            using (var stream = Read(name))
            {
                return stream?.ReadBytes();
            }
        }
        
        /// <summary>
        /// read string from named file
        /// </summary>
        public string ReadString(string name)
        {
            using (var stream = Read(name))
            {
                return stream?.ReadString();
            }
        }

        public Stream Write(string name) => GetStream(name, true);
        
        public TextWriter TextWriter(string name)
        {
            var stream = Write(name);
            return new StreamWriter(stream);
        }

        /// <summary>
        /// write string content to named file
        /// </summary>
        public void WriteString(string name, string content)
        {
            using (var stream = Write(name))
            {
                stream.WriteString(content);
            }
        }
        
        /// <summary>
        /// write source stream to target file
        /// </summary>
        public void WriteStream(string name, Stream source)
        {
            using (var stream = Write(name))
            {
                source.CopyTo(stream);
            }
        }
        
        /// <summary>
        /// write bytes to target file
        /// </summary>
        public void WriteBytes(string name, byte[] bytes)
        {
            using (var stream = Write(name))
            {
                stream.WriteBytes(bytes);
            }
        }

        /// <summary>
        /// move one file to another, source file must exist, target file will be removed if exists
        /// </summary>
        public virtual void Move(string nameFrom, string nameTo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// delete file
        /// </summary>
        public virtual void Delete(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// check if file exists
        /// </summary>
        public virtual bool Exists(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// list all the file names
        /// </summary>
        public virtual List<string> List(Func<string, bool> fileFilter = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// copy all files to target
        /// </summary>
        public void CopyTo(AbstractFileSystem target, Func<string, bool> fileFilter = null)
        {
            var names = List(fileFilter);
            foreach (var name in names)
            {
                using (var stream = Read(name))
                {
                    target.WriteStream(name, stream);
                }
            }
        }

        /// <summary>
        /// delete all the files
        /// </summary>
        public void DeleteAll(Func<string, bool> fileFilter = null)
        {
            var names = List(fileFilter);
            foreach (var name in names)
            {
                Delete(name);
            }
        }
    }
}
