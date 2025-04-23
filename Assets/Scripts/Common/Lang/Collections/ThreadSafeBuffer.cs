using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Common.Lang.Collections
{
    public class ThreadSafeBuffer<T>
    {
        private ConcurrentQueue<T> _queue = new();
        
        private ConcurrentQueue<T> _queueBac = new();

        public void Add(T item)
        {
            _queue.Enqueue(item);
        }

        public void Flush(Action<T> consumer)
        {
            _queueBac = Interlocked.Exchange(ref _queue, _queueBac);
            if (_queueBac.IsEmpty) return;
            foreach (var e in _queueBac)
            {
                consumer(e); 
            }
            _queueBac.Clear();
        }
    }
}
