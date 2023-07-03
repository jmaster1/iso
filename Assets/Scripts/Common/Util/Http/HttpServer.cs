using System;
using System.Net;
using System.Net.Sockets;
using Common.Lang;
using Common.Lang.Entity;
using Common.Util.Threading;

namespace Common.Util.Http
{
    /// <summary>
    /// HttpServer
    /// </summary>
    public class HttpServer : GenericBean, IDisposable
    {
        /// <summary>
        /// port that succeeded to listen
        /// </summary>
        public int Port { get; private set;}
        
        /// <summary>
        /// list of ports to listen, first successful will be used, should be assigned before Start()
        /// </summary>
        public int[] Ports = {9876, 9877, 9878, 9879};
        
        /// <summary>
        /// number of worker threads
        /// </summary>
        public int WorkerThreads = 3;

        /// <summary>
        /// filters will be invoked before handler,
        /// filter may analyse/decorate request/response,
        /// if any filter return false, request wont be processed
        /// </summary>
        public event Func<HttpQuery, bool> Filters;
        
        /// <summary>
        /// client query handler, query must be disposed on end
        /// </summary>
        public Action<HttpQuery> QueryHandler;

        private ThreadedTaskQueue tasks;

        private HttpListener listener = new HttpListener();

        /// <summary>
        /// start listening for incoming connections
        /// </summary>
        public void Start()
        {
            foreach (var p in Ports)
            {
                try
                {
                    listener.Prefixes.Clear();
                    listener.Prefixes.Add($"http://+:{p}/");
                    listener.Start();
                    Port = p;
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Server started on port {0}", Port);
                    }
                    break;
                }
                catch (Exception ex)
                {
                    Log.Warn($"port {p} seems to be busy", ex);
                }
            }
            tasks = new ThreadedTaskQueue(WorkerThreads + 1, "http-");
            tasks.PushTask(AcceptConnections);
        }
        
        public void Dispose()
        {
            Stop();
        }
        
        public void Stop()
        {
            tasks?.Dispose();
            tasks = null;
            listener?.Stop();
            listener = null;
        }

        private void AcceptConnections()
        {
            while (listener.IsListening)
            {
                try
                {
                    var tc = listener.GetContext();
                    tasks.PushTask(() => ServeHttp(tc));
                }
                catch (SocketException)
                {
                    break;
                }
            }
        }

        private void ServeHttp(HttpListenerContext ctx)
        {
            var q = new HttpQuery(ctx); 
            //
            // invoke filters
            var proceed = true;
            if (Filters != null)
            {
                proceed = Filters.Invoke(q);
            }

            if (!proceed)
            {
                q.Dispose();
                return;
            }
            //
            // invoke handler
            QueryHandler(q);
        }
    }
}