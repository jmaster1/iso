using System.IO;
using System.Text;

namespace Common.Util
{
    public static class IoHelper
    {
        /// <summary>
        /// read text from file safely
        /// </summary>
        /// <param name="path"></param>
        /// <returns>null if file not exists</returns>
        public static string ReadFileAsText(params string[] path)
        {
            string text = null;
            var combined = Path.Combine(path);
            if (File.Exists(combined))
            {
                text = File.ReadAllText(combined);
            }
            return text;
        }

        /// <summary>
        /// retrieve MemoryStream from given text using utf-8
        /// </summary>
        /// <param name="text"></param>
        /// <returns>never null</returns>
        public static MemoryStream StringStream(string text)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(text ?? ""));
        }

        public static FileStream ReadFile(string path)
        {
            return File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
        
        public static FileStream WriteFile(string path)
        {
            return File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Write);
        }
    }
}