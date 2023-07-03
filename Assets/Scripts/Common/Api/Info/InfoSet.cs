using System.Collections;
using System.Collections.Generic;
using Common.Lang;
using Common.Lang.Entity;

namespace Common.Api.Info
{
    /// <summary>
    /// represents list of info descriptors
    /// this container is lazy, i.e. data is not loaded until list requested.
    /// Id matches resource name to load data from
    /// </summary>
    public class InfoSet<T> : AbstractEntityIdString, IEnumerable<T>
    {
        /// <summary>
        /// InfoApi that created this
        /// </summary>
        public InfoApi InfoApi;
        
        /// <summary>
        /// list of elements (created on demand)
        /// </summary>
        private List<T> list;

        /// <summary>
        /// list of entities
        /// </summary>
        public List<T> List
        {
            get { return list ?? (list = InfoApi.LoadInfoList<T>(Id)); }
        }

        public int Count => List.Count;

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return List.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return List.GetEnumerator();
        }
        
        public T Get(int i)
        {
            return List[i];
        }
    }
}