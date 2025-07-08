using System.Collections.Concurrent;
using System.Text.Json;
using System.Web;
using Common.IO.Streams;
using Common.IO.Codec;
using IsoNet.Core.Log.Appender;
using IsoNet.Core.Proxy;
using IsoNet.Core.Transport.Rmi;
using Microsoft.Extensions.Logging;

namespace IsoNet.Core.Log;

public class TransportRmiHtmlLogger : AbstractLogger
{
    private static readonly ConcurrentDictionary<int, DateTime> RequestTime = new();

    public static void HtmlStart(IAppender appender)
    {
        appender.Append(HtmlWriter.BuildString(w =>
        {
            w.plain(HtmlLogger.Css);
            w.plain("""
                    <style>
                        .truncate {
                            white-space: nowrap;
                            overflow: hidden;
                            text-overflow: ellipsis;
                            max-width: 500px;
                        }
                    </style>
                    <script>
                        function setHtml(elId, html) {
                            let el = document.getElementById(elId);
                            el.innerHTML = html;
                        }
                        
                        function addHtml(elId, html) {
                            let el = document.getElementById(elId);
                            el.innerHTML += html;
                        }
                        function toggleOverflow(d) {
                            d.classList.toggle("truncate")
                        }
                    </script>
                    """);
            w.table().tr()
                .th("Time")
                .th("Initiator")
                .th("Thread")
                .th("M_Type")
                .th("M_Id")
                .th("Method")
                .th("Req")
                .th("Resp")
                .endTr();
        }));
    }

    public override void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception, string> formatter)
    {
        if (eventId == default) return;
        var messageType = ExtractParam<MessageType, TState>(state, "messageType");
        var requestId = eventId.Id;
        switch (eventId.Name)
        {
            case TransportRmi.NameInvokeRemote:
                var call = ExtractParam<MethodCall, TState>(state, "call");
                var html = HtmlWriter.BuildString(w =>
                {
                    RequestTime[requestId] = DateTime.Now;
                    w.tr().attrClass("log-" + logLevel.ToString().ToLower())
                        .td($"{DateTime.Now:HH:mm:ss.fff}")
                        .td(call.Source + ">" + call.Target)
                        .td($"{Thread.CurrentThread.Name} @ {Environment.CurrentManagedThreadId}")
                        .td(messageType)
                        .td(requestId)
                        .td(call!)
                        .td()
                            .span().attrId(ContainerId(TransportRmi.NameWriteMessage, MessageType.Request, requestId)).endSpan()
                            .span().attrId(ContainerId(TransportRmi.NameReadMessage, MessageType.Request, requestId)).endSpan()
                            .div()
                                .attrId(ContainerId(null, MessageType.Request, requestId))
                                .attrClass("truncate")
                                .attrOnClick("toggleOverflow(this)")
                            .endDiv()
                        .endTd()
                        .td()
                            .span().attrId(ContainerId(TransportRmi.NameWriteMessage, MessageType.Response, requestId)).endSpan()
                            .span().attrId(ContainerId(TransportRmi.NameReadMessage, MessageType.Response, requestId)).endSpan()
                            .div()
                                .attrId(ContainerId(null, MessageType.Response, requestId))
                                .attrClass("truncate")
                                .attrOnClick("toggleOverflow(this)")
                            .endDiv()
                        .endTd()
                        .endTr();
                });
                Append(html);
                break;
            case TransportRmi.NameReadMessage:
            case TransportRmi.NameWriteMessage:
                var requestTime = RequestTime[requestId];
                var timeSpan = DateTime.Now.Subtract(requestTime);
                var id = ContainerId(eventId.Name, messageType, requestId);
                Append($"<script>addHtml('{id}', '{timeSpan.TotalMilliseconds:0} ms')</script>");
                break;
            case LoggingCodec.EventNameRead:
            case LoggingCodec.EventNameWrite:
                var transportRmiEventId = TransportRmiLogContext.GetCurrent();
                var message = ExtractParam<string, TState>(state, "str");
                id = ContainerId(null, transportRmiEventId.MessageType, 
                    transportRmiEventId.EventId.Id);
                Append($"<script>setHtml('{id}', '{HttpUtility.JavaScriptStringEncode(message)}')</script>");
                break;
            case LoggingCodec.EventNameReadError:
            case LoggingCodec.EventNameWriteError:
                transportRmiEventId = TransportRmiLogContext.GetCurrent();
                var ex = ExtractParam<Exception, TState>(state, "ex");
                id = ContainerId(transportRmiEventId.EventId.Name!, transportRmiEventId.MessageType, 
                    transportRmiEventId.EventId.Id);
                var msgEscaped = JsonSerializer.Serialize(ex!.Message).Trim('"');
                Append($"<script>addHtml('{id}', '<div class=\"error\">{msgEscaped}</div>')</script>");
                break;
        }
    }

    private static string ContainerId(string? operationName, MessageType messageType, int requestId)
    {
        if (messageType == MessageType.Call)
        {
            messageType = MessageType.Request;
        }
        return $"{operationName}_{messageType}_{requestId}";
    }
}