using System;
using Common.Api.Pool;

namespace Common.Lang.Observable
{

    public class PooledObsList<T> : ObsList<T> where T : class
    {
        private readonly Pool<T> pool = new();
        
        public T PooledAdd(Action<T> initializer)
        {
            var e = pool.Get();
            initializer(e);
            Add(e);
            return e;
        }

        public void PooledRemove(T element)
        {
            Validate(Contains(element));
            Remove(element);
            pool.Put(element);
        }
    }
}
