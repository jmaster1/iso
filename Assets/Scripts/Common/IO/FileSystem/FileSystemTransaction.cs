using System.Collections.Generic;
using System.IO;
using Common.Lang;
using Common.Lang.Collections;

namespace Common.IO.FileSystem
{
    /// <summary>
    /// guarantees data atomicity/consistency/durability of filesystem data
    /// that has been written between begin/end calls. When transaction starts,
    /// transaction log file created (LogFileName), each file being written 
    /// is written into temp file, which name constructed using pattern {originalName}.{TempSuffix},
    /// it's name added to log file as new line.
    /// When transaction finished, LogEntryEnd line written to log file,
    /// then each temp file moved to original file, eventually log file removed.
    /// Upon filesystem creation it is necessary to invoke Check() method,
    /// to ensure completion of incomplete transaction (if any).
    /// </summary>
    public class FileSystemTransaction : AbstractFileSystem
    {
        /// <summary>
        /// log file name, this must be unique for entire filesystem
        /// </summary>
        private static readonly string LogFileName = "$$$.txt";
        
        /// <summary>
        /// suffix for temporary files
        /// </summary>
        private static readonly string TempSuffix = ".$$$";
        
        /// <summary>
        /// this line is written to transaction log on End() call,
        /// this should be invalid file name text
        /// </summary>
        private static readonly string LogEntryEnd = "*";

        /// <summary>
        /// underlying provider
        /// </summary>
        AbstractFileSystem provider;
        
        /// <summary>
        /// file names used in current transaction (mapping name > tempName)
        /// </summary>
        private readonly Map<string, string> names = new Map<string, string>();

        /// <summary>
        /// transaction log writer
        /// </summary>
        private StreamWriter logWriter;

        public bool InProgress => logWriter != null;

        public FileSystemTransaction(AbstractFileSystem provider)
        {
            this.provider = provider;
        }

        /// <summary>
        /// invoke this to begin transaction
        /// </summary>
        public void Begin()
        {
            if (Log.IsDebugEnabled) Log.Debug("Begin");
            Validate(!InProgress);
            var logStream = provider.Write(LogFileName);
            logWriter = new StreamWriter(logStream);
        }

        /// <summary>
        /// invoke this to end transaction
        /// </summary>
        public void End()
        {
            if (Log.IsDebugEnabled) Log.Debug("End");
            Validate(InProgress);
            logWriter.WriteLine(LogEntryEnd);
            logWriter.Close();
            logWriter = null;
            foreach (var kv in names)
            {
                var name = kv.Key;
                var nameTmp = kv.Value;
                if (Log.IsDebugEnabled) Log.DebugFormat("Moving {0} > {1}", nameTmp, name);
                provider.Move(nameTmp, name);
            }
            names.Clear();
            provider.Delete(LogFileName);
        }

        string GetTempName(string name)
        {
            return name + TempSuffix;
        }

        public override Stream GetStream(string name, bool write)
        {
            if (write && InProgress)
            {
                var nameTmp = GetTempName(name);
                names.Add(name, nameTmp);
                logWriter.WriteLine(name);
                if (Log.IsDebugEnabled) Log.DebugFormat("Writing {0} > {1}", name, nameTmp);
                name = nameTmp;
            }
            return provider.GetStream(name, write);
        }

        public override long GetLength(string name)
        {
            return provider.GetLength(name);
        }

        /// <summary>
        /// this should be called upon FileSystem construction to ensure previously
        /// incomplete transaction finish 
        /// </summary>
        public void Check() {
            Validate(!InProgress);
            var logStream = provider.Read(LogFileName);
            if(logStream == null) return;
            using (var logReader = new StreamReader(logStream))
            {
                var names = new List<string>();
                var end = false;
                for (;;)
                {
                    var line = logReader.ReadLine();
                    if (line == null) break;
                    if (LogEntryEnd.Equals(line))
                    {
                        end = true;
                        break;
                    }
                    names.Add(line);
                }

                if (!end) return;
                foreach (var name in names)
                {
                    var nameTmp = GetTempName(name);
                    if (provider.Exists(nameTmp))
                    {
                        provider.Move(nameTmp, name);
                    }
                }
                provider.Delete(LogFileName);
            }
        }
    }
}