using System;
using System.Globalization;
using Common.Lang;
using Common.Lang.Collections;

namespace Common.Util
{
    /// <summary>
    /// compatible of parsing text to value of arbitrary type
    /// </summary>
    public class TextParser
    {
        /// <summary>
        /// prefix for hexadecimal int format
        /// </summary>
        public static readonly string PrefixHex = "0x";
        
        /// <summary>
        /// default instance
        /// </summary>
        public static TextParser Instance = new TextParser();
        
        /// <summary>
        /// parsers mapped by type
        /// </summary>
        public Map<Type, Func<string, object>> Parsers = new Map<Type, Func<string, object>>();

        public TextParser()
        {
            ApplyDefaults();
        }

        /// <summary>
        /// parse text to value of specified type
        /// </summary>
        public object Parse(string text, Type type)
        {
            if (text == null) return null;
            //
            // special case for enum
            if (type.IsEnum)
            {
                return Enum.Parse(type, text);
            }
            //
            // special case for array
            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var split = text.Split(',');
                var n = split.Length;
                var array = Array.CreateInstance(elementType, n);
                for (var i = 0; i < n; i++)
                {
                    var elementText = split[i].Trim();
                    var e = Parse(elementText, elementType);
                    array.SetValue(e, i);
                }
                return array;
            }
            var parser = Parsers.Get(type);
            var val = parser(text);
            return val;
        }

        public T Parse<T>(string text)
        {
            var type = typeof(T);
            return (T) Parse(text, type);
        }

        public void ApplyDefaults()
        {
            Parsers.Add(typeof(bool), ParseBool);
            Parsers.Add(typeof(float), ParseFloat);
            Parsers.Add(typeof(int), ParseInt);
            Parsers.Add(typeof(uint), ParseUInt);
            Parsers.Add(typeof(string), ParseString);
        }
        
        public static object ParseBool(string text)
        {
            switch (text)
            {
                case "1":
                    return true;
                case "0":
                    return false;
                default:
                    return bool.Parse(text);
            }
        }
        
        public static object ParseFloat(string text)
        {
            return float.Parse(text, NumberStyles.Float | NumberStyles.AllowThousands, 
                NumberFormatInfo.InvariantInfo);
        }

        public static object ParseInt(string text)
        {
            if (text.IsNullOrEmpty()) return 0;
            return text.StartsWith(PrefixHex) ? Convert.ToInt32(text,16) : int.Parse(text);
        }
        
        public static object ParseUInt(string text)
        {
            if (text.IsNullOrEmpty()) return 0;
            return text.StartsWith(PrefixHex) ? Convert.ToUInt32(text,16) : uint.Parse(text);
        }
        
        public static object ParseString(string text)
        {
            return text;
        }
    }
}