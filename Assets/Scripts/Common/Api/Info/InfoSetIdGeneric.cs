using Common.Lang;

namespace Common.Api.Info
{
    /// <summary>
    /// InfoSet extension that supports mapping by element id (of generic type)
    /// </summary>
    public class InfoSetIdGeneric<T, TID> : InfoSet<T> where T : IIdAware<TID>
    {
        /// <summary>
        /// entities mapped by id
        /// </summary>
        protected IdAwareMap<TID, T> map;

        public IdAwareMap<TID, T> Map
        {
            get
            {
                if (map != null) return map;
                var n = List.Count;
                map = new IdAwareMap<TID, T>(n);
                foreach (var e in List)
                {
                    map.Add(e);
                }
                return map;
            }
        }

        public T FindById(TID id)
        {
            return id == null ? default : Map.Find(id);
        }
        
        public T GetById(TID id)
        {
            return Map.Get(id);
        }

        TID EntityToId(T entity)
        {
            return entity.GetId();
        }
    }
}
