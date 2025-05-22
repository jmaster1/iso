using Common.Lang.Observable;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using IsoNet.Core.Transport;
using IsoNet.Core.Transport.Rmi;
using IsoNetTest.Core;
using IsoNetTest.Core.Log;
using Microsoft.Extensions.Logging;

namespace IsoNetTest.Common;

public class TransportRmiTests : AbstractTests
{
    private class SimpleBean
    {
        public string? ValueString;
        
        public int ValueInt;
    }
        
    private interface ITestApi
    {
        [Call]
        void CallVoid();

        [Query]
        Task<string> QueryStringAsync();
        
        [Query]
        string QueryString();
        
        [Query]
        void QueryThrows();
        
        [Query]
        SimpleBean QuerySimpleBean(string stringValue, int intValue);
        
        [Query]
        Task<SimpleBean> QuerySimpleBeanAsync(string stringValue, int intValue);
        
        [Query]
        SimpleBean QuerySimpleBeanThrows(string stringValue, int intValue);
        
        [Query]
        Task<SimpleBean> QuerySimpleBeanAsyncThrows(string stringValue, int intValue);
    }
    
    private enum TestApiEvent
    {
        Inv
    }

    private class TestApiImpl : ITestApi
    {
        public Events<TestApiEvent, string> Events = new();

        private void Fire(string name)
        {
            Events.Fire(TestApiEvent.Inv, name);
        }
        
        public void CallVoid()
        {
            Fire(nameof(CallVoid));
        }

        public Task<string> QueryStringAsync()
        {
            Fire(nameof(QueryStringAsync));
            return Task.FromResult(nameof(QueryStringAsync));
        }

        public string QueryString()
        {
            Fire(nameof(QueryString));
            return nameof(QueryString);
        }

        public void QueryThrows()
        {
            Fire(nameof(QueryThrows));
            throw new NotImplementedException();
        }

        public SimpleBean QuerySimpleBean(string stringValue, int intValue)
        {
            Fire(nameof(QuerySimpleBean));
            return new SimpleBean
            {
                ValueString = stringValue,
                ValueInt = intValue
            };
        }

        public Task<SimpleBean> QuerySimpleBeanAsync(string stringValue, int intValue)
        {
            Fire(nameof(QuerySimpleBeanAsync));
            return Task.FromResult(new SimpleBean
            {
                ValueString = stringValue,
                ValueInt = intValue
            });
        }

        public SimpleBean QuerySimpleBeanThrows(string stringValue, int intValue)
        {
            Fire(nameof(QuerySimpleBeanThrows));
            throw new NotImplementedException();
        }

        public Task<SimpleBean> QuerySimpleBeanAsyncThrows(string stringValue, int intValue)
        {
            Fire(nameof(QuerySimpleBeanAsyncThrows));
            throw new NotImplementedException();
        }
    }
    
    private ITestApi apiClnRemote;
    
    private ITestApi apiSrvRemote;
    
    private TestApiImpl apiSrvLocal;
    
    private TestApiImpl apiClnLocal;
    
    private Func<string, TaskCompletionSource<string>> ServerMethodInvoked;
    
    protected override void ConfigureLoggingBuilder(ILoggingBuilder builder)
    {
        builder.AddProvider(TransportRmiHtmlLogger.Provider);
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var (transportCln, transportSrv) = LocalTransport.CreatePair();

        var codec = new JsonCodec()
            .AddConverter(MethodCallJsonConverter.Instance)
            .AddConverter(new ExceptionJsonConverter());

        var rmiSrv = new TransportRmi(transportSrv, codec.WrapLogging(CreateLogger("srv")))
        {
            Logger = CreateLogger("rmiSrv")
        };
        apiSrvRemote = rmiSrv.CreateRemote<ITestApi>();
        apiSrvLocal = new TestApiImpl();
        rmiSrv.RegisterLocal<ITestApi>(apiSrvLocal);

        var rmiCln = new TransportRmi(transportCln, codec.WrapLogging(CreateLogger("cln")))
        {
            Logger = CreateLogger("rmiCln"),
            RequestIdOffset = 1000
        };
        apiClnRemote = rmiCln.CreateRemote<ITestApi>();
        apiClnLocal = new TestApiImpl();
        rmiCln.RegisterLocal<ITestApi>(apiClnLocal);

        ServerMethodInvoked = name => CreateTaskCompletionSource(apiSrvLocal.Events, TestApiEvent.Inv, name);
    }
    
    [TearDown]
    public void Teardown()
    {
    }
    
    [Test]
    public async Task Test2()
    {
        for (int i = 0; i < 10; i++)
        {
            apiClnRemote.QueryString();
            apiSrvRemote.QueryString();    
        }
    }
    
    [Test]
    public async Task QuerySimpleBeanAsyncThrows_ThrowsNotImplemented()
    {
        await TestNotImplementedAsync(() => apiClnRemote.QuerySimpleBeanAsyncThrows("123", 321));
    }

    [Test]
    public async Task QueryStringAsync_ReturnsExpectedValue()
    {
        var invoked = ServerMethodInvoked(nameof(ITestApi.QueryStringAsync));
        var result = await apiClnRemote.QueryStringAsync();
        Assert.That(result, Is.EqualTo(nameof(ITestApi.QueryStringAsync)));
        await AwaitResult(invoked);
    }

    [Test]
    public void QuerySimpleBean_ReturnsCorrectValues()
    {
        var result = apiClnRemote.QuerySimpleBean("123", 321);
        Assert.That(result.ValueString, Is.EqualTo("123"));
        Assert.That(result.ValueInt, Is.EqualTo(321));
    }

    [Test]
    public async Task QuerySimpleBeanAsync_ReturnsCorrectValues()
    {
        var result = await apiClnRemote.QuerySimpleBeanAsync("123", 321);
        Assert.That(result.ValueString, Is.EqualTo("123"));
        Assert.That(result.ValueInt, Is.EqualTo(321));
    }

    [Test]
    public void QuerySimpleBeanThrows_ThrowsNotImplemented()
    {
        TestNotImplemented(() => apiClnRemote.QuerySimpleBeanThrows("123", 321));
    }

    [Test]
    public void QueryThrows_ThrowsNotImplementedAndIsInvoked()
    {
        TestNotImplemented(() => apiClnRemote.QueryThrows());
    }

    [Test]
    public async Task CallVoid_IsInvoked()
    {
        var invoked = ServerMethodInvoked(nameof(ITestApi.CallVoid));
        apiClnRemote.CallVoid();
        await AwaitResult(invoked);
    }

    [Test]
    public async Task QueryString_ReturnsExpectedValueAndIsInvoked()
    {
        var invoked = ServerMethodInvoked(nameof(ITestApi.QueryString));
        var result = apiClnRemote.QueryString();
        Assert.That(result, Is.EqualTo(nameof(ITestApi.QueryString)));
        await AwaitResult(invoked);
    }
}
