using System.Collections.Concurrent;

namespace IsoNet.Core.Pool;

public class ConcurrentBagPool<T> : IPool<T> where T : new()
{
    private readonly ConcurrentBag<T> _objects = [];
    
    public T Get()
    {
        return _objects.TryTake(out var item) ? item : new T();
    }

    public void Return(T item)
    {
        _objects.Add(item);
    }
}
