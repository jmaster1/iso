using Common.IO.Streams;

namespace Common.Util.Http
{
    /// <summary>
    /// request/response processor split by methods
    /// </summary>
    public interface IHttpQueryProcessor
    {
        void OnHttpRequest(HttpQuery request);
        
        void OnHttpResponse(HttpQuery query, HtmlWriter html);
    }
}