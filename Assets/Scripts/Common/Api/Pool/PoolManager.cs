using System;
using System.Text;
using Common.Lang;
using Common.Lang.Collections;
using Common.Lang.Entity;
using Common.Util;

namespace Common.Api.Pool
{
    /// <summary>
    /// pool manager maintains pools for arbitrary types
    /// </summary>
    public class PoolManager : GenericBean, IObjectFactory, IObjectReceiver
    {
        /// <summary>
        /// pool cache mapped by type
        /// </summary>
        private readonly Map<Type, object> pools = new();

        private Pool<object> GetPool<T>() where T : class
        {
            return GetPool(typeof(T));
        }
        
        public Pool<object> GetPool(Type type)
        {
            Assert(type != typeof(object));
            var pool = pools.Find(type);
            if (pool == null)
            {
                pools[type] = pool = new Pool<object>(type);
            }
            return (Pool<object>) pool;
        }

        public T Create<T>() where T : class
        {
            var pool = GetPool<T>();
            return (T) pool.Get();
        }

        public void Put(object obj)
        {
            Assert(obj != null);
            var type = obj!.GetType();
            var pool = GetPool(type);
            pool.Put(obj);
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var v in pools.Values)
            {
                sb.Append(v).Append(StringHelper.EOL);
            }
            return sb.ToString();
        }
    }
}