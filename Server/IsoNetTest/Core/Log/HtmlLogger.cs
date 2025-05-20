using Common.IO.Streams;
using Microsoft.Extensions.Logging;

namespace IsoNetTest.Core.Log;

public class HtmlLogger(string category) : AbstractLogger
{
    public static readonly ILoggerProvider Provider = new LoggerProvider(
        category => new HtmlLogger(category));
    
    private static readonly string LogFilePath = FileAppender.LogFilePath("test-log.html", HtmlStart);

    private static string HtmlStart()
    {
        return HtmlWriter.BuildString(w =>
        {
            w.plain("""
                    <style>
                    html *
                    {
                       font-size: 1em !important;
                       color: #000 !important;
                       font-family: Verdana !important;
                    }
                    table, th, td {
                    	border: 1px solid #ccc; /* или #ddd, #999 — по вкусу */
                    	border-collapse: collapse; /* чтобы границы не дублировались */
                    }
                    
                    th, td {
                    	padding: 4px 8px;
                    }
                    
                    tr.log-trace {
                      background-color: #f9f9f9;
                      color: #666;
                    }
                    
                    tr.log-debug {
                      background-color: #eef5ff;
                      color: #335;
                    }
                    
                    tr.log-information {
                      background-color: #e8f5e9;
                      color: #2e7d32;
                    }
                    
                    tr.log-warning {
                      background-color: #fff8e1;
                      color: #f57c00;
                    }
                    
                    tr.log-error {
                      background-color: #ffebee;
                      color: #c62828;
                      font-weight: bold;
                    }
                    
                    tr.log-critical {
                      background-color: #fbe9e7;
                      color: #b71c1c;
                      font-weight: bold;
                      border-left: 4px solid #b71c1c;
                    }
                    
                    </style>
                    """);
            w.table().tr()
                .th("Time")
                .th("Level")
                .th("Category")
                .th("Thread")
                .th("Message")
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
        var msg = formatter(state, exception);
        var html = HtmlWriter.BuildString(w =>
        {
            w.tr().attrClass("log-" + logLevel.ToString().ToLower())
                .td($"{DateTime.Now:HH:mm:ss.fff}")
                .td(logLevel)
                .td(category)
                .td($"{Thread.CurrentThread.Name} @ {Environment.CurrentManagedThreadId}")
                .td(msg)
                .endTr();
        });
        FileAppender.Append(LogFilePath, html);
    }
}