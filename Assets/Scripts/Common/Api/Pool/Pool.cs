using System;
using System.Collections.Generic;
using Common.Util;
using Common.Lang;
using Common.Util.Reflect;

namespace Common.Api.Pool
{
    /// <summary>
    /// container for pooled objects supposed to use in release mode
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pool<T> : IIdAware<Type>, IClearable where T : class
    {
        public List<T> FreeObjects { get; } = new List<T>();

        /// <summary>
        /// element type
        /// </summary>
        private Type type;
        
        /// <summary>
        /// element factory, use reflection to instantiate if not provided
        /// </summary>
        private Func<T> factory;

        /// <summary>
        /// pool size retrieval (free objects)
        /// </summary>
        public int Size => FreeObjects.Count;

        public Pool(Type type, Func<T> factory = null)
        {
            this.type = type;
            this.factory = factory;
        }
        
        public Pool() : this(typeof(T)) {
        }

        /// <summary>
        /// borrow object from the pool, may create new if not available
        /// </summary>
        /// <returns>
        /// object from pool or create new if not available
        /// </returns>
        public virtual T Get()
        {
            T result = FreeObjects.Pop() ?? Create();
            return result;
        }

        protected virtual T Create()
        {
            if (factory != null)
            {
                return factory.Invoke();
            }

            return (T) ReflectHelper.NewInstance(type);
        }

        /// <summary>
        /// object release
        /// </summary>
        /// <param name="value">target object</param>
        public virtual void Put(T value)
        {
            LangHelper.Validate(!FreeObjects.Contains(value));
            if (value is IClearable clearable) clearable.Clear();
            FreeObjects.Add(value);
        }

        /// <summary>
        /// put all the objects from list into this pool and clear list
        /// </summary>
        public virtual void PutAll(IList<T> values)
        {
            for (var i = values.Count - 1; i >= 0; --i)
            {
                Put(values[i]);
            }
            values.Clear();
        }

        /// <summary>
        /// put all the objects from array into this pool and clear array
        /// </summary>
        public virtual void PutAll(T[] values)
        {
            if (values == null) return;
            foreach (var value in values)
            {
                Put(value);
            }
        }

        /// <summary>
        /// clear free objects (to release memory)
        /// </summary>
        public virtual void Clear()
        {
            foreach (var e in FreeObjects)
            {
                if (e is IDisposable disposable) disposable.Dispose();
            }
            FreeObjects.Clear();
        }

        /// <summary>
        /// check if object contains in pool free objects
        /// </summary>
        /// <param name="value">target value</param>
        /// <returns>'true' if contains</returns>
        public virtual bool Contains(T value) => FreeObjects.Contains(value);

        public Type GetId()
        {
            return type;
        }

        public override string ToString()
        {
            return typeof(T).Name + "=" + Size;
        }
    }
}
