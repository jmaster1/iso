using System;

namespace Common.Lang.Observable
{
    /// <summary>
    /// observable event publisher.
    /// published events are:
    /// - identified by enum (T), so should be handled by subscribers using switch
    /// - supplemented with payload (P), which contains event specific data
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    /// <typeparam name="TPayload"></typeparam>
    public class Events<TType, TPayload> where TType: Enum
    {
        private readonly Listeners<Action<TType, TPayload>> listeners = new Listeners<Action<TType, TPayload>>();

        public int Size => listeners.Size;

        public bool IsFiring => listeners.Started;
        
        /// <summary>
        /// publish event to subscribers
        /// </summary>
        /// <param name="type"></param>
        /// <param name="payload"></param>
        public void Fire(TType type, TPayload payload)
        {
            int i = 0, n = listeners.Begin();
            for(; i < n; i++)
            {
                var listener = listeners.Get(i);
                listener(type, payload);
            }
            listeners.End();
        }

        public void AddListener(Action<TType, TPayload> e)
        {
            listeners.Add(e);
        }
        
        public void RemoveListener(Action<TType, TPayload> e)
        {
            listeners.Remove(e);
        }
    }
}
