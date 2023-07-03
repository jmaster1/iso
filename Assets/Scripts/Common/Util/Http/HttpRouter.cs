using System;
using System.Collections.Generic;
using System.Linq;
using Common.Bind;
using Common.ContextNS;
using Common.IO.Streams;
using Common.Lang;

namespace Common.Util.Http
{
    /// <summary>
    /// routes http processing for handlers mapped by path
    /// </summary>
    public class HttpRouter : BindableBean<HttpServer>
    {
        /// <summary>
        /// handlers mapped by path
        /// </summary>
        readonly Map<string, HttpHandlerDetails> handlers = new Map<string, HttpHandlerDetails>();
        
        /// <summary>
        /// filter to invoke request handling
        /// </summary>
        public Func<HttpQuery, bool> filter;
        
        public void AddHandler(IHttpQueryProcessor processor, Type type = null, string name = null, string path = null, string group = null)
        {
            Validate(processor != null || type != null);
            if (type == null) type = processor.GetType();
            if(name == null) name = type.Name;
            if(path == null) path = name;
            if (group == null)
            {
                var ns = type.Namespace;
                if (ns != null)
                {
                    group = ns.BeforeFirst(".");
                }

                if (group == null)
                {
                    group = StringHelper.EmptyString;
                }
            }

            var details = new HttpHandlerDetails
            {
                Handler = processor,
                HandlerType = type,
                Path = path,
                Name = name,
                Group = group
            };
            handlers[path] = details;
        }
        
        /// <summary>
        /// add handler from type instantiated via context
        /// </summary>
        public void AddHandler<T>() where T : IHttpQueryProcessor
        {
            AddHandler(null, typeof(T));
        }
        
        public void RemoveHandler(IHttpQueryProcessor handler)
        {
            handlers.RemoveValues(details => details.Handler == handler);
        }
        
        public void HandleQuery(HttpQuery query)
        {
            if (Log.IsDebugEnabled) Log.Debug($"HandleQuery: {query}");
            if (filter != null && !filter(query))
            {
                if (Log.IsDebugEnabled) Log.Debug($"Filter rejected: {query}");
                return;
            }
            try
            {
                var split = query.RequestPathSplit;
                var handlerDetails = split.IsEmpty() ? null : handlers.Find(split[0]);
                if (handlerDetails == null)
                {
                    RenderHandlers(query);
                    return;
                }
                //
                // instantiate handler on demand
                var handler = handlerDetails.Handler ?? (handlerDetails.Handler =
                    Context.GetCurrent().GetBean<IHttpQueryProcessor>(handlerDetails.HandlerType));
                HttpInvokeHandler.HandleCommand(query, handler);
                if (query.Disposed) return;
                handler.OnHttpRequest(query);
                if (!query.IsContentTypeSet)
                {
                    query.SetContentTypeHtml();
                    RenderPageHeader(query.html, handler);
                }

                handler.OnHttpResponse(query, query.html);
            }
            catch (Exception ex)
            {
                Log.Error("HandleRequest() Failed", ex);
                RenderErrorPage(ex, query);
            }
            finally
            {
                query.Dispose();
            }
        }

        void RenderErrorPage(Exception ex, HttpQuery query)
        {
            var html = query.html;
            query.SetContentTypeHtml();
            html.h1("Error");
            html.textarea("error", 120, 10).plain(ex.ToString()).end();
        }

        /// <summary>
        /// render handlers directory
        /// </summary>
        void RenderHandlers(HttpQuery query)
        {
            query.SetContentTypeHtml();
            var html = query.html;
            var list = handlers.Values.ToList();
            list.Sort();
            string lastGroup = null;
            foreach (var e in list)
            {
                if (!StringHelper.Equals(lastGroup, e.Group))
                {
                    if (lastGroup != null)
                    {
                        html.endUl();
                    }
                    lastGroup = e.Group;
                    html.h2(lastGroup);
                    html.ul();
                    
                }
                html.li().a(e.Path, e.Name).endLi();
            }
            if (lastGroup != null)
            {
                html.endUl();
            }
            
        }

        /// <summary>
        /// render html page header in handler context
        /// </summary>
        void RenderPageHeader(HtmlWriter html, IHttpQueryProcessor handler)
        {
            var type = handler.GetType();
            html.table().tr()
                .td().h1(type.Name).endTd()
                .td().commandsForm(HttpInvokeHandler.CmdRefresh).endTd().td();
            HttpInvokeHandler.RenderHttpInvokeMethods(html, handler);
            html.endTd().endTr().endTable();
        }

        public void AddHandlers(IEnumerable<IHttpQueryProcessor> list, string group = null)
        {
            foreach (var handler in list)
            {
                AddHandler(handler, @group: group);
            }
        }
    }

    internal class HttpHandlerDetails : IComparable<HttpHandlerDetails>
    {
        public IHttpQueryProcessor Handler;

        public Type HandlerType;

        public string Name;
        
        public string Path;

        public string Group;

        public int CompareTo(HttpHandlerDetails other)
        {
            var ret = StringHelper.Compare(Group, other.Group);
            return ret == 0 ? StringHelper.Compare(Name, other.Name) : ret;
        }
    }
}
