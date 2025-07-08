using Common.IO.Streams;

namespace Common.Util.Http
{
    /// <summary>
    /// request/response processor split by methods
    /// </summary>
    public interface IHttpQueryProcessor
    {
        void OnHttpRequest(HttpQuery query);
        
        void OnHttpResponse(HttpQuery query, HtmlWriter html);
    }
}