using System;
using System.Collections.Generic;
using Common.Lang;
using Common.Util;

namespace Common.Api.Info
{
    /// <summary>
    /// info (static configuration data) provider api
    /// </summary>
    public class InfoApi : AbstractApi
    {
        /// <summary>
        /// folder name containing info files 
        /// </summary>
        public static string Folder = "Info";
        
        /// <summary>
        /// resource loaders, will try in order until non-null result
        /// </summary>
        public readonly List<Func<string, Type, object>> loaders = new();

        /// <summary>
        /// info cache
        /// </summary>
        CacheString<object> cache = new();

        /// <summary>
        /// load named resource
        /// </summary>
        private object Load(string name, Type type)
        {
            var path = Folder + StringHelper.FileSeparator + name;
            var n = loaders.Count;
            if(n == 0) LangHelper.Throw("No loaders provided");
            for (var i = 0; i < n; i++)
            {
                var loader = loaders[i];
                var ret = loader(path, type);
                if (ret != null) return ret;
            }
            LangHelper.Throw($"Unable to load {name} of type {type}");
            return null;
        }

        private T Load<T>(String name)
        {
            if(Log.IsDebugEnabled) Log.Debug($"Load({name})");
            var type = typeof(T);
            var ret = (T) Load(name, type);
            return ret;
        }
        
        /// <summary>
        /// load list of entities from specified resource file
        /// </summary>
        public List<T> LoadInfoList<T>(string name)
        {
            return Load<List<T>>(name);
        }

        /// <summary>
        /// retrieve InfoSetId of specified type that should be loaded from named resource
        /// </summary>
        public InfoSet<T> GetInfoSet<T>(string name)
        {
            if (cache.Find(name, out InfoSet<T> ret)) return ret;
            ret = new InfoSet<T>();
            ret.Id = name;
            ret.InfoApi = this;
            cache.Put(name, ret);
            return ret;
        }
        
        /// <summary>
        /// retrieve InfoSetId of specified type that should be loaded from named resource
        /// </summary>
        public InfoSetIdString<T> GetInfoSetIdString<T>(string name) where T : IIdAware<string>
        {
            if (cache.Find(name, out InfoSetIdString<T> ret)) return ret;
            ret = new InfoSetIdString<T>();
            ret.Id = name;
            ret.InfoApi = this;
            cache.Put(name, ret);
            return ret;
        }
        
        public InfoSetIdGeneric<T, ID> GetInfoSetIdGeneric<T, ID>(string name) where T : IIdAware<ID>
        {
            if (cache.Find(name, out InfoSetIdGeneric<T, ID> ret)) return ret;
            ret = new InfoSetIdGeneric<T, ID>();
            ret.Id = name;
            ret.InfoApi = this;
            cache.Put(name, ret);
            return ret;
        }
        
        /// <summary>
        /// retrieve info descriptor of specified type from named resource,
        /// if name not set, will use type name for that
        /// </summary>
        public T GetInfo<T>(string name = null) where T: class
        {
            if (name == null)
            {
                var type = typeof(T);
                name = type.Name;
            }
            if (cache.Find(name, out T ret)) return ret;
            ret = Load<T>(name);
            cache.Put(name, ret);
            return ret;
        }
    }
}