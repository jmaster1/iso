using System;
using System.Collections.Generic;
using Common.Api.Info;
using Common.Lang;
using Common.Lang.Entity;
using Common.Util;
using Common.Util.Reflect;

namespace Common.ContextNS
{
    /// <summary>
    /// bean container
    /// </summary>
    public class Context : GenericBean
    {
        private static Context instance;
        
        /// <summary>
        /// instantiated singletons
        /// </summary>
        private readonly Map<Type, object> beans = new Map<Type, object>();

        public event Action<Type, object> OnBeanCreated; 
        
        /// <summary>
        /// current (local) context retrieval
        /// </summary>
        /// <returns></returns>
        public static Context GetCurrent()
        {
            return instance ?? (instance = new Context());
        }
        
        /// <summary>
        /// retrieve bean of specified type
        /// </summary>
        /// <typeparam name="T">type of bean</typeparam>
        /// <returns></returns>
        public T GetBean<T>()
        {
            return GetBean<T>(typeof(T));
        }
        
        public T GetBean<T>(Type type)
        {
            if (beans.TryGetValue(type, out var bean)) return (T) bean;
            if(Log.IsDebugEnabled) Log.Debug($"Creating bean of type {type.Name}");
            bean = ReflectHelper.NewInstance<T>();
            beans[type] = bean;
            OnBeanCreated?.Invoke(type, bean);
            return (T)bean;
        }

        public static T Get<T>()
        {
            return GetCurrent().GetBean<T>();
        }

        /// <summary>
        /// put bean for specified base type 
        /// </summary>
        /// <param name="derivedObject"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool PutBean<T>(T derivedObject)
        {
            if(Log.IsDebugEnabled) Log.Debug($"PutBean: {derivedObject}");
            var type = typeof(T);
            if (!beans.ContainsKey(type))
            {
                beans.Add(type, derivedObject);
                return true;
            }

            return false;
        }

        public Dictionary<Type, object>.ValueCollection GetBeans()
        {
            return beans.Values;
        }
        
        public static bool Put<T>(T derivedObject)
        {
            return GetCurrent().PutBean(derivedObject);
        }

        /// <summary>
        /// shortcut to InfoApi.GetInfoSet()
        /// </summary>
        public new static InfoSet<T> GetInfoSet<T>(string name)
        {
            return Get<InfoApi>().GetInfoSet<T>(name);
        }
        
        /// <summary>
        /// shortcut to InfoApi.GetInfoSetIdString()
        /// </summary>
        public static InfoSetIdString<T> GetInfoSetIdString<T>(string name) where T : IIdAware<string>
        {
            return Get<InfoApi>().GetInfoSetIdString<T>(name);
        }
        
        /// <summary>
        /// shortcut to InfoApi.GetInfoSetIdGeneric()
        /// </summary>
        public static InfoSetIdGeneric<T, TID> GetInfoSetIdGeneric<T, TID>(string name) where T : IIdAware<TID>
        {
            return Get<InfoApi>().GetInfoSetIdGeneric<T, TID>(name);
        }
        
        /// <summary>
        /// shortcut to InfoApi.GetInfo()
        /// </summary>
        public static T GetInfo<T>(string resource = null) where T: class
        {
            return Get<InfoApi>().GetInfo<T>(resource);
        }
    }
}
