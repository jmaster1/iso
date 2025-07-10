using System;
using System.Collections.Generic;
using System.Linq;
using Common.Bind;
using Common.ContextNS;
using Common.IO.Streams;
using Common.Lang.Collections;

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
        private readonly Map<string, HttpHandlerDetails> _handlers = new();
        
        /// <summary>
        /// filter to invoke request handling
        /// </summary>
        public Func<HttpQuery, bool> Filter;
        
        public void AddHandler(IHttpQueryProcessor processor, 
            Type type = null, 
            string name = null, 
            string path = null, 
            string group = null)
        {
            Validate(processor != null || type != null);
            type ??= processor!.GetTargetType();
            name ??= type.Name;
            path ??= name;
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
            _handlers[path] = details;
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
            _handlers.RemoveValues(details => details.Handler == handler);
        }
        
        public void HandleQuery(HttpQuery query)
        {
            if (Log.IsDebugEnabled) Log.Debug($"HandleQuery: {query}");
            if (Filter != null && !Filter(query))
            {
                if (Log.IsDebugEnabled) Log.Debug($"Filter rejected: {query}");
                query.Dispose();
                return;
            }
            try
            {
                var split = query.RequestPathSplit;
                var handlerDetails = split.IsEmpty() ? null : _handlers.Find(split[0]);
                if (handlerDetails == null)
                {
                    query.DoAndDispose(() => RenderHandlers(query));
                    return;
                }
                //
                // instantiate handler on demand
                var handler = handlerDetails.Handler ?? (handlerDetails.Handler =
                    Context.GetCurrent().GetBean<IHttpQueryProcessor>(handlerDetails.HandlerType));

                void Continuation(Action postHeaderAction)
                {
                    try
                    {
                        handler.OnHttpRequest(query);
                        if (query.Disposed) return;
                        if (!query.IsContentTypeSet)
                        {
                            query.SetContentTypeHtml();
                            RenderPageHeader(query.Html, handler);
                            postHeaderAction?.Invoke();
                            handler.RenderMethods(query);
                        }

                        handler.OnHttpResponse(query, query.Html);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "HandleRequest() Failed");
                        RenderErrorPage(ex, query);
                    }
                    finally
                    {
                        query.Dispose();
                    }
                }

                HttpInvokeHandler.HandleCommand(query, handler, Continuation);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "HandleRequest() Failed");
                RenderErrorPage(ex, query);
            }
        }

        private void RenderErrorPage(Exception ex, HttpQuery query)
        {
            var html = query.Html;
            query.SetContentTypeHtml();
            html.h1("Error");
            html.textarea("error", 120, 10).plain(ex.ToString()).end();
        }

        /// <summary>
        /// render handlers directory
        /// </summary>
        private void RenderHandlers(HttpQuery query)
        {
            query.SetContentTypeHtml();
            var html = query.Html;
            var list = _handlers.Values.ToList();
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
        private void RenderPageHeader(HtmlWriter html, IHttpQueryProcessor handler)
        {
            var type = handler.GetTargetType();
            html.table().tr()
                .td().h1(type.Name).endTd()
                .td().commandsForm(HttpInvokeHandler.CmdRefresh).endTd()
                .endTr().endTable();
        }

        public void AddHandlers(IEnumerable<IHttpQueryProcessor> list, string group = null)
        {
            foreach (var handler in list)
            {
                AddHandler(handler, group: group);
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
