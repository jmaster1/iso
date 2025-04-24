using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Common.Lang.Collections
{
    public class ThreadSafeBuffer<T>
    {
        private ConcurrentQueue<T> _queueA = new();
        
        private ConcurrentQueue<T> _queueB = new();
        
        private volatile ConcurrentQueue<T> _current = null!;

        public ThreadSafeBuffer()
        {
            _current = _queueA;
        }
        
        public void Add(T item)
        {
            _current.Enqueue(item);
        }

        public void Flush(Action<T> consumer)
        {
            var previous = Interlocked.Exchange(ref _current, _current == _queueA ? _queueB : _queueA);
            while (previous.TryDequeue(out var item))
            {
                consumer(item);
            }
        }
    }
}
