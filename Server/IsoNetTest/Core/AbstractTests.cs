using Common.Bind;
using Common.Lang.Observable;
using IsoNetTest.Core.Log;
using Microsoft.Extensions.Logging;

namespace IsoNetTest.Core;

public abstract class AbstractTests
{
    public ILoggerFactory TestLoggerFactory { get; set; }

    protected ILogger Logger;
    
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        TestLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .SetMinimumLevel(LogLevel.Debug)
                .AddProvider(TestContextLogger.Provider)
                .AddProvider(HtmlLogger.Provider);
            ConfigureLoggingBuilder(builder);
        });
        Logger = CreateLogger(this);
    }

    protected virtual void ConfigureLoggingBuilder(ILoggingBuilder builder)
    {
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        TestLoggerFactory.Dispose();
    }

    protected ILogger CreateLogger(string category)
    {
        return TestLoggerFactory.CreateLogger(category);
    }
    
    protected ILogger CreateLogger(object instance)
    {
        return CreateLogger(instance.GetType().Name);
    }

    protected static TaskCompletionSource<TPayload> CreateTaskCompletionSource<TPayload>(
        Action<TaskCompletionSource<TPayload>> setTaskCompletionAction)
    {
        var taskCompletionSource = new TaskCompletionSource<TPayload>();
        setTaskCompletionAction.Invoke(taskCompletionSource);
        return taskCompletionSource;
    }
    
    protected static TaskCompletionSource<TPayload> CreateTaskCompletionSource<TEvent, TPayload>(
        Events<TEvent, TPayload> events, TEvent expectedEvent, TPayload? expectedPayload = default) where TEvent : Enum
    {
        var taskCompletionSource = new TaskCompletionSource<TPayload>();
        events.AddListener((eventType, payload) =>
        {
            if(eventType.Equals(expectedEvent) && (expectedPayload == null || payload!.Equals(expectedPayload)))
            {
                taskCompletionSource.TrySetResult(payload);
            }
        });
        return taskCompletionSource;
    }
    
    protected static TaskCompletionSource<TValue> CreateTaskCompletionSource<TValue>(BindableBean<TValue> bindable)
    {
        var taskCompletionSource = new TaskCompletionSource<TValue>();
        bindable.ModelHolder.AddListener((_, _, newVal) =>
        {
            taskCompletionSource.TrySetResult(newVal);
        });
        return taskCompletionSource;
    }
    
    protected static TaskCompletionSource<TValue> CreateTaskCompletionSource<TValue>(Holder<TValue> holder)
    {
        var taskCompletionSource = new TaskCompletionSource<TValue>();
        holder.AddListener((_, newVal, _) =>
        {
            taskCompletionSource.TrySetResult(newVal);
        });
        return taskCompletionSource;
    }
    
    protected static async Task<T> AwaitResult<T>(TaskCompletionSource<T> tcs, float timeoutSec = 1)
    {
        /* this code doesnt work with debug breakpoints
        var timeout = TimeSpan.FromSeconds(timeoutSec);
        var result = await tcs.Task.WaitAsync(timeout);
        return result;
        */
        double accumulatedTimeMs = 0;
        var timeoutMs = timeoutSec * 1000;
        var intervalMs = 50;

        var timer = new System.Timers.Timer(intervalMs);
        timer.AutoReset = true;
        timer.Elapsed += (s, e) =>
        {
            accumulatedTimeMs += intervalMs;
        };
        timer.Start();

        try
        {
            while (!tcs.Task.IsCompleted)
            {
                await Task.Delay(intervalMs);

                if (accumulatedTimeMs >= timeoutMs)
                    throw new TimeoutException("Operation timed out.");
            }

            return await tcs.Task;
        }
        finally
        {
            timer.Stop();
            timer.Dispose();
        }
    }
    
    public class MultiSource<TSource>(params TSource[] sources)
    {
        public MultiTaskCompletionSource<TSource, TResult> CreateTaskCompletionSource<TResult>(
            Func<TSource, TaskCompletionSource<TResult>> func)
        {
            var sourceTasks = new List<Tuple<TSource, TaskCompletionSource<TResult>>>();
            foreach (var source in sources)
            {
                var taskCompletionSource = func(source);
                sourceTasks.Add(new Tuple<TSource, TaskCompletionSource<TResult>>(source, taskCompletionSource));
            }
            return new MultiTaskCompletionSource<TSource, TResult>(sourceTasks);
        }
    }

    public class MultiTaskCompletionSource<TSource, TResult>(
        List<Tuple<TSource, TaskCompletionSource<TResult>>> sourceTasks)
    {
        public async Task AwaitResults(Action<TSource, TResult>? action = null)
        {
            foreach (var sourceTask in sourceTasks)
            {
                var result = await AwaitResult(sourceTask.Item2);
                action?.Invoke(sourceTask.Item1, result);
            }
        }
    }
    
    protected void TestNotImplemented(Action func)
    {
        try
        {
            func();
            Assert.Fail("Expected NotImplementedException");
        }
        catch (NotImplementedException)
        {
        }
    }
    
    protected async Task TestNotImplementedAsync(Func<Task> func)
    {
        try
        {
            await func();
            Assert.Fail("Expected NotImplementedException");
        }
        catch (NotImplementedException)
        {
        }
    }
}
