using Common.Lang.Observable;
using Microsoft.Extensions.Logging;

namespace IsoNetTest;

public abstract class AbstractTests
{
    public ILoggerFactory TestLoggerFactory { get; set; }

    protected ILogger Logger;
    
    [SetUp]
    public void Setup()
    {
        TestLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .SetMinimumLevel(LogLevel.Debug)
                .AddProvider(new TestContextLoggerProvider());
        });
        Logger = CreateLogger(this);
    }

    [TearDown]
    public void Dispose()
    {
        TestLoggerFactory?.Dispose();
    }

    protected ILogger CreateLogger(string category)
    {
        return TestLoggerFactory.CreateLogger(category);
    }
    
    protected ILogger CreateLogger(object instance)
    {
        return CreateLogger(instance.GetType().Name);
    }
    
    protected static TaskCompletionSource<TPayload> CreateTaskCompletionSource<TEvent, TPayload>(
        Events<TEvent, TPayload> events, TEvent expectedEvent) where TEvent : Enum
    {
        var taskCompletionSource = new TaskCompletionSource<TPayload>();
        events.AddListener((eventType, payload) =>
        {
            if(eventType.Equals(expectedEvent))
            {
                taskCompletionSource.TrySetResult(payload);
            }
        });
        return taskCompletionSource;
    }
    
    protected static async Task<T> AwaitResult<T>(TaskCompletionSource<T> tcs, float timeoutSec = 1)
    {
        var timeout = TimeSpan.FromSeconds(timeoutSec);
        var result = await tcs.Task.WaitAsync(timeout);
        return result;
    }
}
