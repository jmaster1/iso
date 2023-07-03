using System;
using System.Text;
using Common.Api.Resource;
using Common.ContextNS;
using Common.Lang;
using Common.Lang.Observable;
using Common.Util;
using Common.Util.Http;

namespace Common.Api.Local
{
    /// <summary>
    /// localization api
    /// </summary>
    public class LocalApi : AbstractApi
    {
        /// <summary>
        /// code for english language (default one)
        /// </summary>
        public const string LangEn = "en";
        
        /// <summary>
        /// separator used for joining keys
        /// </summary>
        public const char KeySeparator = '.';

        /// <summary>
        /// folder name containing localization files 
        /// </summary>
        public const string Folder = "Local";

        /// <summary>
        /// prefix to use for message resources file name, suffix should be language code
        /// </summary>
        public const string FileNamePrefix = "messages_";

        private readonly ResourceApi resourceApi = Context.Get<ResourceApi>();

        /// <summary>
        /// current messages table (never null, but might be uninitialized)
        /// </summary>
        private readonly MessagesTable table = new MessagesTable();

        /// <summary>
        /// what to return on message retrieval
        /// </summary>
        [HttpInvoke]
        public MessageResolutionPolicy MessageResolutionPolicy = MessageResolutionPolicy.ValueOrNull;

        public string Lang => table.Id;

        /// <summary>
        /// currently selected language
        /// </summary>
        public readonly StringHolder Language = new StringHolder();

        /// <summary>
        /// retrieve resource file name for given language
        /// </summary>
        public static string GetMessagesFileName(string lang)
        {
            return FileNamePrefix + lang;
        }
        
        /// <summary>
        /// check if language set
        /// </summary>
        public bool IsLanguageSet()
        {
            return table.Id != null;
        }

        /// <summary>
        /// change language (load localizations)
        /// </summary>
        [HttpInvoke]
        public void SetLanguage(string lang)
        {
            if(StringHelper.Equals(Lang, lang)) return;
            if (Log.IsDebugEnabled) Log.DebugFormat("SetLanguage({0})", lang);
            var name = GetMessagesFileName(lang);
            var path = Folder + StringHelper.FileSeparator + name;
            using (var reader = resourceApi.ReadResource(path))
            {
                table.Read(reader);
            }

            Language.Set(lang);
        }

        public string GetKey(string key0, string key1 = null, string key2 = null, string key3 = null,
            string key4 = null)
        {
            var sb = new StringBuilder();
            sb.Append(key0);
            if (key1 != null)
            {
                sb.Append(KeySeparator).Append(key1);
                if (key2 != null)
                {
                    sb.Append(KeySeparator).Append(key2);
                    if (key3 != null)
                    {
                        sb.Append(KeySeparator).Append(key3);
                        if (key4 != null)
                        {
                            sb.Append(KeySeparator).Append(key4);
                        }
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// retrieve message for chain of keys, result key evaluated by joining keys with separator
        /// </summary>
        public string GetMessage(string key0, string key1 = null, string key2 = null, string key3 = null, string key4 = null)
        {
            Validate(table.Map != null, "Messages not loaded, call SetLanguage() first");
            if (MessageResolutionPolicy == MessageResolutionPolicy.Key)
            {
                return GetKey(key0, key1, key2, key3, key4);
            }
            var hash = StringHelper.Hash(out _, KeySeparator, key0, key1, key2, key3, key4);
            table.Map.TryGetValue(hash, out var result);
            if (result != null) return result;
            switch (MessageResolutionPolicy)
            {
                case MessageResolutionPolicy.ValueOrNull:
                    return null;
                case MessageResolutionPolicy.ValueOrError:
                    throw new Exception("Message not found: " + GetKey(key0, key1, key2, key3, key4));
                case MessageResolutionPolicy.ValueOrKey:
                    return GetKey(key0, key1, key2, key3, key4);
            }
            return null;
        }

        /// <summary>
        /// retrieve key for specified type 
        /// </summary>
        public string GetTypeKey(Type type)
        {
            return type.Name;
        }
        
        public string GetTypeKey<T>()
        {
            var type = typeof(T);
            return GetTypeKey(type);
        }
        
        /// <summary>
        /// retrieve key for specified object 
        /// </summary>
        /// <returns>object type name</returns>
        public string GetObjectKey(object obj)
        {
            var type = obj.GetType();
            return type.Name;
        }
        
        /// <summary>
        /// retrieve message for object property, the key is constructed using object type name and property.
        /// </summary>
        /// <param name="obj">required to obtain type name as key prefix</param>
        /// <returns></returns>
        public string GetObjectMessage(object obj, string key0, string key1 = null, string key2 = null)
        {
            var name = GetObjectKey(obj);
            return GetMessage(name, key0, key1, key2);
        }
        
        /// <summary>
        /// retrieve message for enum constant, the key is constructed as enum type name and enum value name
        /// </summary>
        public string GetEnumMessage(Enum obj, string suffix0 = null, string suffix1 = null)
        {
            var type = obj.GetType();
            return GetMessage(type.Name, obj.ToString(), suffix0, suffix1);
        }

        /// <summary>
        /// retrieve number of existing key variations for given base,
        /// variations are expected to be formatted using pattern:
        /// {key0}...{keyN}.{X}, where X - number > 0
        /// </summary>
        public int GetVariationCount(string key0, string key1 = null, string key2 = null, string key3 = null)
        {
            var hash = StringHelper.Hash(out var nullKeyIndex, KeySeparator, key0, key1, key2, key3);
            string key4 = null;
            for (var i = 1;; i++)
            {
                var key = i.ToStr();
                    i.ToStr();
                switch (nullKeyIndex)
                {
                    case -1:
                        key4 = key;
                        break;
                    case 0:
                        key0 = i.ToStr();
                        break;
                    case 1:
                        key1 = i.ToStr();
                        break;
                    case 2:
                        key2 = i.ToStr();
                        break;
                    case 3:
                        key3 = i.ToStr();
                        break;
                }
                var message = GetMessage(key0, key1, key2, key3, key4);
                if (message == null)
                {
                    return i - 1;
                }
            }
        }
    }
}