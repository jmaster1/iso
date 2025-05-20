using System.Collections.Concurrent;
using Common.IO.Streams;
using IsoNet.Core.Proxy;
using IsoNet.Core.Transport.Rmi;
using Microsoft.Extensions.Logging;

namespace IsoNetTest.Core.Log;

public class TransportRmiHtmlLogger(string category) : AbstractLogger
{
    public static readonly ILoggerProvider Provider = new LoggerProvider(
        category => new TransportRmiHtmlLogger(category));
    
    private static readonly ConcurrentDictionary<int, DateTime> RequestTime = new();
    
    private static readonly string LogFilePath = FileAppender.LogFilePath("rmi-log.html", HtmlStart);
    
    private static string HtmlStart()
    {
        return HtmlWriter.BuildString(w =>
        {
            w.plain(HtmlLogger.Css);
            w.plain("""
                    <script>
                    function setHtml(elId, html) {
                    	let el = document.getElementById(elId);
                    	el.innerHTML = html;
                    }
                    function addHtml(elId, html) {
                    	let el = document.getElementById(elId);
                    	el.innerHTML += html;
                    }
                    </script>
                    """);
            w.table().tr()
                .th("Time")
                .th("Initiator")
                .th("Thread")
                .th("MessageType")
                .th("RequestId")
                .th("Method")
                .th("Call>>")
                .th(">>Call")
                .th("Req>>")
                .th(">>Req")
                .th("Resp>>")
                .th("<<Resp")
                .endTr();
        });
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
                        .td(category)
                        .td($"{Thread.CurrentThread.Name} @ {Environment.CurrentManagedThreadId}")
                        .td(messageType)
                        .td(requestId)
                        .td(call!)
                        .td().attrId(Id(TransportRmi.NameWriteMessage, MessageType.Call, requestId)).endTd()
                        .td().attrId(Id(TransportRmi.NameReadMessage, MessageType.Call, requestId)).endTd()
                        .td().attrId(Id(TransportRmi.NameWriteMessage, MessageType.Request, requestId)).endTd()
                        .td().attrId(Id(TransportRmi.NameReadMessage, MessageType.Request, requestId)).endTd()
                        .td().attrId(Id(TransportRmi.NameWriteMessage, MessageType.Response, requestId)).endTd()
                        .td().attrId(Id(TransportRmi.NameReadMessage, MessageType.Response, requestId)).endTd()
                        .endTr();
                });
                FileAppender.Append(LogFilePath, html);
                break;
            case TransportRmi.NameReadMessage:
            case TransportRmi.NameWriteMessage:
                var requestTime = RequestTime[requestId];
                var timeSpan = DateTime.Now.Subtract(requestTime);
                var id = Id(eventId.Name, messageType, requestId);
                FileAppender.Append(LogFilePath, 
                    $"<script>addHtml('{id}', '{timeSpan.TotalMilliseconds:0}')</script>");
                break;
        }
    }

    private string Id(string operationName, MessageType messageType, int requestId)
    {
        return $"{operationName}_{messageType}_{requestId}";
    }
}