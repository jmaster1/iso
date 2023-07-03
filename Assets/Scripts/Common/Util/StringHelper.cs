using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Common.Lang;

namespace Common.Util
{
    /// <summary>
    /// string manipulation utility
    /// </summary>
    public static class StringHelper
    {
        public const string Space = " ";
        
        public const string FileSeparator = "/";
        
        public const string EOL = "\r\n";

        public static readonly Encoding UTF8 = Encoding.UTF8;
        
        public const string EmptyString = "";

        public static bool IsNullOrEmpty(this string src)
        {
            return string.IsNullOrEmpty(src);
        }
        
        public static bool StartsWith(string val, string prefix)
        {
            return val != null && val.StartsWith(prefix);
        }

        public static bool EqualsIgnoreCase(string v0, string v1)
        {
            if (v0 == null && v1 == null)
            {
                return true;
            }
            if (v0 == null || v1 == null)
            {
                return false;
            }
            return string.Equals(v0, v1, StringComparison.InvariantCultureIgnoreCase);
        }
        
        public static bool Equals(string v0, string v1)
        {
            if (v0 == null && v1 == null)
            {
                return true;
            }
            if (v0 == null || v1 == null)
            {
                return false;
            }
            return string.Equals(v0, v1, StringComparison.InvariantCulture);
        }
        
        public static int Compare(string v0, string v1)
        {
            if (v0 == null && v1 == null) return 0;
            if (v0 == null) return -1;
            if (v1 == null) return 1;
            return string.Compare(v0, v1, StringComparison.InvariantCulture);
        }

        /// <summary>
        /// retrieve string hash code. we shouldn't use string.GetHashCode() because
        /// it is dependent on its implementation, which might change from one version of the common language runtime to another
        /// </summary>
        /// <param name="key"></param>
        /// <param name="separator"></param>
        /// <param name="h">base hash, used to evaluate hash of joined strings</param>
        /// <returns></returns>
        public static int Hash(string key, char separator = '\0', int h = 0)
        {
            if (key == null)
            {
                return h;
            }
            if (h != 0 && separator != 0)
            {
                h = 31 * h + separator;
            }
            for (int i = 0, n = key.Length; i < n; i++)
            {
                h = 31 * h + key[i];
            }
            return h;
        }
        
        /// <summary>
        /// hash sequence of keys using separator
        /// </summary>
        /// <param name="nullKeyIndex">receives index of first null key, -1 if all not null</param>
        /// <param name="separator">a separator to use as key delimiter (affects evaluated hash)</param>
        public static int Hash(out int nullKeyIndex, char separator, 
            string key0, 
            string key1 = null, 
            string key2 = null, 
            string key3 = null, 
            string key4 = null)
        {
            var hash = 0;
            nullKeyIndex = 0;
            if (key0 == null) return hash;
            nullKeyIndex++;
            hash = Hash(key0, separator, hash);
            if (key1 == null) return hash;
            nullKeyIndex++;
            hash = Hash(key1, separator, hash);
            if (key2 == null) return hash;
            nullKeyIndex++;
            hash = Hash(key2, separator, hash);
            if (key3 == null) return hash;
            nullKeyIndex++;
            hash = Hash(key3, separator, hash);
            if (key4 == null) return hash;
            nullKeyIndex = -1;
            hash = Hash(key4, separator, hash);
            return hash;
        }

        public static int Hash(char separator, 
            string key0, 
            string key1 = null,
            string key2 = null, 
            string key3 = null, 
            string key4 = null)
        {
            return Hash(out var nullKeyIndex, separator, key0, key1, key2, key3, key4);
        }

        public static string ToString(double v)
        {
            return v.ToString(CultureInfo.InvariantCulture);
        }

        public static int IndexOf(string source, string find)
        {
            return source?.IndexOf(find, StringComparison.Ordinal) ?? -1;
        }

        /// <summary>
        /// formatted string retrieval
        /// </summary>
        public static string Format(string format, 
            object arg0,
            object arg1 = null, 
            object arg2 = null,
            object arg3 = null, 
            object arg4 = null)
        {
            return string.Format(format, arg0, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// max int (exclusive) which text representation might be cached
        /// </summary>
        const int IntMaxToString = 256;
        
        /// <summary>
        /// constructed string representation of int that matches element index
        /// </summary>
        private static readonly string[] IntStrings = new string[IntMaxToString];

        public static string ToStr(this int val)
        {
            if (val >= IntMaxToString || val < 0)
            {
                return val.ToString();
            }
            var ret = IntStrings[val] ?? (IntStrings[val] = val.ToString());
            return ret;
        }

        public static string ToStr(this long val)
        {
            if (val >= IntMaxToString || val < 0)
            {
                return val.ToString();
            }
            return ((int) val).ToStr();
        }

        /// <summary>
        /// convert string to stream using UTF-8
        /// </summary>
        /// <param name="str"></param>
        /// <returns>null if source is null</returns>
        public static Stream GetStream(string str)
        {
            if (str == null) return null;
            var bytes = UTF8.GetBytes(str);
            return new MemoryStream(bytes);
        }

        /// <summary>
        /// read all stream context as string using UTF-8
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <returns>null if stream is null</returns>
        public static string GetString(Stream stream, Encoding encoding = null)
        {
            if (stream == null) return null;
            var reader = new StreamReader(stream, encoding ?? UTF8);
            var text = reader.ReadToEnd();
            return text;
        }

        public static StringBuilder BeginList(this StringBuilder sb)
        {
            return sb.Append("[");
        }
        
        public static StringBuilder ElementSeparator(this StringBuilder sb)
        {
            if (sb.Length == 0 || sb[sb.Length - 1] == '[') return sb;
            return sb.Append(", ");
        }
        
        public static StringBuilder EndList(this StringBuilder sb)
        {
            return sb.Append("]");
        }

        public static string Join(IEnumerable<object> elements, char separator = ',')
        {
            var sb = new StringBuilder();
            foreach (var e in elements)
            {
                if (sb.Length > 0) sb.Append(separator);
                var val = e;
                if (e is IIdAware<string> idAware) val = idAware.GetId();
                sb.Append(val);
            }

            return sb.ToString();
        }
        
        public static bool IsDigitsOnly(this string str)
        {
            return str.All(c => c >= '0' && c <= '9');
        }

        public static StringBuilder AppendProperties(this StringBuilder sb, 
            params object[] namesAndValues)
        {
            sb.BeginList();
            for (var i = 0; i < namesAndValues.Length;)
            {
                var name = namesAndValues[i++];
                var value = namesAndValues[i++];
                sb.ElementSeparator().Append(name).Append("=").Append(value);
            }
            sb.EndList();
            return sb;
        }

        public const string DefaultSeparator = ", ";
        
        public static string Join<T>(this T[] array, string separator = DefaultSeparator)
        {
            if (array == null || array.Length == 0) return null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                if (sb.Length > 0) sb.Append(separator);
                sb.Append(array[i]);
            }
            return sb.ToString();
        }

        public static string BeforeFirst(this string src, string token)
        {
            var index = src.IndexOf(token, StringComparison.InvariantCulture);
            return index == -1 ? src : src.Substring(0, index);
        }
        
        public static bool Contains(this string src, char c)
        {
            return src != null && src.IndexOf(c) != -1;
        }

        public static StringBuilder AppendKeyValueLine(this StringBuilder sb, object key, object value, string separator = "=")
        {
            return sb.Append(key).Append(separator).Append(value).Append(EOL);
        }
    }
}
