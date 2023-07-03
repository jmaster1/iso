using System;
using Common.Util.Log;

namespace Common.Util
{
    /// <summary>
    /// singleton wrapper, sample usage as type fields:
    /// private static readonly Singleton&lt;T&gt; Singleton = new Singleton&lt;T&gt;(FactoryMethod);
    /// public static T Instance => _singleton.Get();
    /// </summary>
    /// <typeparam name="T">type of singleton</typeparam>
    public class Singleton<T> where T : class
    {
        private LogWrapper log;
        
        public LogWrapper Log => log ?? (log = new LogWrapper(GetType()));
        
        private T instance;

        private readonly Func<T> factory;
        
        /// <summary>
        /// shows whether instantiation is in progress
        /// </summary>
        public bool Instantiating { get; private set; }
        
        /// <summary>
        /// instantiation error
        /// </summary>
        public Exception Error { get; private set; }
        
        public Singleton(Func<T> factory)
        {
            this.factory = factory;
        }

        public T Get()
        {
            if (instance != null) return instance;
            if (Error != null) return null;
            LangHelper.Validate(!Instantiating);
            Instantiating = true;
            try
            {
                instance = factory();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to instantiate {typeof(T).Name}", ex);
                Error = ex;
            }
            finally
            {
                Instantiating = false;
            }
            return instance;
        }
    }
}