using System;
using Common.IO.Streams;

namespace Common.Util.Http
{
    public class TargetHttpQueryProcessor<T> : IHttpQueryProcessor
    {
        private T _target;
    
        public TargetHttpQueryProcessor(T target)
        {
            _target = target;
        }

        public object GetTarget()
        {
            return _target;
        }

        public Type GetTargetType()
        {
            return typeof(T);
        }

        public void RenderMethods(HttpQuery query)
        {
            HttpInvokeHandler.RenderMethods(query, query.Html, this);
        }
    }

}