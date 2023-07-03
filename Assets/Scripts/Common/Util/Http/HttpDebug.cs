using System.Linq;
using Common.Bind;
using Common.ContextNS;

namespace Common.Util.Http
{
    /// <summary>
    /// context http debug
    /// </summary>
    public class HttpDebug : BindableBean<Context>
    {
        public HttpServer Server = new HttpServer();

        public HttpRouter Router = new HttpRouter();

        protected override void OnBind()
        {
            //
            // register router in context
            Model.PutBean(Router);
            //
            // HttpServer
            Server.QueryHandler = Router.HandleQuery;
            Server.Start();
            //
            // context beans
            var beans = Model.GetBeans();
            foreach (var handler in beans.OfType<IHttpQueryProcessor>())
            {
                Router.AddHandler(handler);
            }

            Model.OnBeanCreated += (type, bean) =>
            {
                if (bean is IHttpQueryProcessor handler) Router.AddHandler(handler);
            };
        }
    }
}
