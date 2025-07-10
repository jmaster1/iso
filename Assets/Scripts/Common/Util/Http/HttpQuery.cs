using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Common.IO.Streams;
using Common.Lang.Collections;

namespace Common.Util.Http
{
    /// <summary>
    /// http request/response wrapper
    /// </summary>
    public class HttpQuery : IDisposable
    {
        public const char PathSeparator = '/';
        
        public bool Disposed { get; private set; }
            
        private readonly HttpListenerContext _context;
        
        public HttpListenerRequest Request => _context.Request;
        
        public HttpListenerResponse Response => _context.Response;

        public NameValueCollection RequestHeaders => Request.Headers;
        
        /// <summary>
        /// request path (without query string), never null,
        /// for root query it is '/'
        /// </summary>
        public string RequestPath { get; }
        
        /// <summary>
        /// split of RequestPath, never null, empty array for root request
        /// </summary>
        public string[] RequestPathSplit { get; }

        /// <summary>
        /// request parameters mapped by name, value is string either List of string
        /// </summary>
        private readonly Map<string, object> _requestParameters = new();
        
        public WebHeaderCollection ResponseHeaders => Response.Headers;
        
        /// <summary>
        /// stream for output
        /// </summary>
        public Stream OutputStream => Response.OutputStream;

        /// <summary>
        /// writer for output
        /// </summary>
        public readonly TextWriter Writer;
        
        /// <summary>
        /// html writer
        /// </summary>
        public readonly HtmlWriter Html;
        
        public HttpQuery(HttpListenerContext ctx)
        {
            _context = ctx;
            Writer = new StreamWriter(OutputStream);
            Html = new HtmlWriter(Writer);
            RequestPath = HttpUtility.UrlDecode(Request.Url.AbsolutePath);
            RequestPathSplit = RequestPath.Split(PathSeparator).Where(e => !e.IsNullOrEmpty()).ToArray();
            ParseRequestParameters();
        }

        public override string ToString()
        {
            return Request?.Url?.ToString();
        }

        private void ParseRequestParameters()
        {
            var query = Request.Url.Query;
            if (query.IsNullOrEmpty()) return;
            var keyValues = query[1..].Split('&');
            foreach (var kv in keyValues)
            {
                var index = kv.IndexOf('=');
                var key = kv[..index];
                key = WebUtility.UrlDecode(key);
                var value = kv[(index + 1)..];
                value = WebUtility.UrlDecode(value);
                var existingValue = _requestParameters.Find(key);
                switch (existingValue)
                {
                    case null:
                        _requestParameters[key] = value;
                        break;
                    case string str:
                        var list = new List<string>();
                        list.Add(str);
                        list.Add(value);
                        _requestParameters[key] = list;
                        break;
                    case List<string> strings:
                        strings.Add(value);
                        break;
                }
            }
        }

        public void Dispose()
        {
            if(Disposed) return;
            Html.Flush();
            OutputStream.Close();
            Disposed = true;
        }
       
        public string GetParameter(string name)
        {
            var val = _requestParameters.Find(name);
            return val switch
            {
                string s => s,
                List<string> strings => strings[0],
                _ => null
            };
        }
        
        public T GetEnum<T>(string name, T def = default) where T : Enum
        {
            var str = GetParameter(name);
            return str.IsNullOrEmpty() ? def : LangHelper.EnumParse<T>(str);
        }
        
        public float GetFloat(string name, float def = default)
        {
            var str = GetParameter(name);
            return str.IsNullOrEmpty() ? def : float.Parse(str);
        }
        
        public int GetInt(string name, int def = default)
        {
            var str = GetParameter(name);
            return str.IsNullOrEmpty() ? def : int.Parse(str);
        }

        public string GetCmd()
        {
            return GetParameter(HttpConst.PARAM_CMD);
        }
        
        public void SetContentType(string contentType)
        {
            Response.ContentType = contentType;
        }
        
        public bool IsContentTypeSet => ResponseHeaders.Get(HttpConst.ContentType) != null;

        public void SetContentTypeHtml()
        {
            SetContentType(HttpConst.TextHtml);
        }

        public void SetFileName(string fileName)
        {
            ResponseHeaders[HttpConst.ContentDisposition] = $"inline; filename=\"{fileName}\"";
        }
    }
}