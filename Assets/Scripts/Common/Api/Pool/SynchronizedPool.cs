namespace Common.Api.Pool
{
    public class SynchronizedPool<T> : Pool<T> where T : class
    {
        public override T Get()
        {
            lock (FreeObjects)
            {
                return base.Get();
            }
        }

        public override void Put(T value)
        {
            lock (FreeObjects)
            {
                base.Put(value);
            }
        }
    }
}