using System;
using System.Reflection;
using System.Threading.Tasks;
using Common.IO.Streams;
using Common.Util.Reflect;

namespace Common.Util.Http
{
    /// <summary>
    /// responsible for handling http invocations
    /// </summary>
    public static class HttpInvokeHandler
    {
        private const BindingFlags MethodBindingFlags = BindingFlags.Instance |
                                                        BindingFlags.NonPublic |
                                                        BindingFlags.Public;

        public const string ParamPath = "path";
        public const string CmdRefresh = "Refresh";

        public static void RenderMethods(HttpQuery query, HtmlWriter html, 
            object bean,
            Func<MethodInfo, bool> methodFilter = null,
            object parent = null)
        {
            
            var path = ReflectHelper.ResolvePath(parent, bean);
            var type = bean.GetType();
            if (bean is IHttpQueryProcessor processor)
            {
                type = processor.GetTargetType();
            }
            var methods = type.GetMethods(MethodBindingFlags);
            foreach (var method in methods)
            {
                var proceed = methodFilter == null || methodFilter(method);
                if(!proceed) continue;
                RenderMethod(query, method, path, html);
            }
        }
        
        public static void RenderHttpInvokeMethods(HttpQuery query, HtmlWriter html, object bean, 
            object parent = null)
        {
            RenderMethods(query, html, bean, method => 
                method.GetCustomAttribute<HttpInvokeAttribute>() != null);
        }

        private static void RenderMethod(HttpQuery query, MethodBase method, string path, HtmlWriter html)
        {
            html.form()
                .attrStyle("display:inline; margin:0; padding:0;")
                .inputHidden(ParamPath, path);
            var parameters = method.GetParameters();
            foreach (var parameter in parameters)
            {
                var type = parameter.ParameterType;
                if(type == typeof(HttpQuery))
                {
                    continue;
                }
                var value = query?.GetParameter(parameter.Name);
                html.inputText(parameter.Name, value, "placeholder", parameter.Name);
            }
            html.submitCmd(method.Name).endForm().hr();
        }

        /// <summary>
        /// handle request command in handler context
        /// </summary>
        public static void HandleCommand(HttpQuery query, IHttpQueryProcessor processor, Action<Action> continuation)
        {
            //
            //
            var cmd = query.GetCmd();
            if (cmd.IsNullOrEmpty())
            {
                continuation(null);
                return;
            }
            /*
            var path = query.GetParameter(ParamPath);
            var target = ReflectHelper.ResolveObject(parent, path);
            if(target == null) return;
            */
            
            if (CmdRefresh.Equals(cmd))
            {
                continuation(null);
                return;
            }
            
            var type = processor.GetTargetType();
            var method = type.GetMethod(cmd, ReflectHelper.DefaultBindingFlags);
            LangHelper.Validate(method != null, () => $"method {cmd} not found for {type.Name}");
            //
            // parse args
            var args = PrepareMethodArgs(method, query);
            try
            {
                var target = processor.GetTarget();
                var result = method.Invoke(target, args);
                if (result is Task task)
                {
                    task.ContinueWith((task1, taskResult) =>
                    {
                        var renderResult = task.IsFaulted ? task1.Exception.InnerExceptions[0] : result;
                        continuation(() => query.Html.pre(renderResult));
                    }, null);
                }
                else
                {
                    continuation(() => query.Html.pre(result));
                }
            }
            catch (Exception ex)
            {
                continuation(() => query.Html.pre(ex));
            }
        }

        /// <summary>
        /// prepare method arguments by transforming name matching request parameters
        /// into method arguments. also method may have arguments of type:
        /// - HttpQuery
        /// </summary>
        /// <param name="method"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private static object[] PrepareMethodArgs(MethodBase method, HttpQuery query)
        {
            object[] args = null;
            var parameters = method.GetParameters();
            if (parameters.IsNotEmpty())
            {
                var n = parameters.Length;
                args = new object[n];
                for (var i = 0; i < n; i++)
                {
                    var parameter = parameters[i];
                    var parameterName = parameter.Name;
                    var parameterType = parameter.ParameterType;
                    object val = null;
                    if(parameterType == typeof(HttpQuery))
                    {
                        val = query;
                    }
                    else
                    {
                        var text = query.GetParameter(parameterName);
                        val = TextParser.Instance.Parse(text, parameter.ParameterType);
                    }

                    args[i] = val;
                }
            }

            return args;
        }
    }
}
