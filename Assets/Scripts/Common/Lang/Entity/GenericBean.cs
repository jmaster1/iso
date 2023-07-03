using Common.Api.Info;
using Common.ContextNS;
using Common.IO.Streams;
using Common.Util.Http;
using Common.Util.Log;

namespace Common.Lang.Entity
{
    /// <summary>
    /// base class for beans with:
    /// - shortcuts to context functionality
    /// - logging support
    /// - http processing support
    /// </summary>
    public class GenericBean : AbstractEntityIdString, IHttpQueryProcessor
    {
        private LogWrapper log;

        public LogWrapper Log => log ?? (log = new LogWrapper(GetType()));

        protected static T GetInfo<T>(string resource) where T: class
        {
            return Context.GetInfo<T>(resource);
        }
        
        protected static InfoSet<T> GetInfoSet<T>(string resource)
        {
            return Context.GetInfoSet<T>(resource);
        }
        
        protected static InfoSetIdString<T> GetInfoSetIdString<T>(string resource) where T : IIdAware<string>
        {
            return Context.GetInfoSetIdString<T>(resource);
        }
        
        protected static T GetBean<T>()
        {
            return Context.Get<T>();
        }
        
        public virtual void OnHttpRequest(HttpQuery query)
        {
        }

        public virtual void OnHttpResponse(HttpQuery query, HtmlWriter html)
        {
        }
    }
}
