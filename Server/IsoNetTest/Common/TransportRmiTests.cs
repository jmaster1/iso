using Common.Lang.Observable;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using IsoNet.Core.Transport;
using IsoNet.Core.Transport.Rmi;
using IsoNetTest.Core;
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
        
        [Query]
        int QueryNested(int depth);
    }
    
    private enum TestApiEvent
    {
        Inv
    }

    private class TestApiImpl : ITestApi
    {
        public readonly Events<TestApiEvent, string> Events = new();

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

        public ITestApi QueryNestedDelegate;
        
        public int QueryNested(int depth)
        {
            return depth == 0 ? 0 : QueryNestedDelegate.QueryNested(depth - 1);
        }
    }

    class Endpoint
    {
        public ITestApi ApiRemote;
    
        public TestApiImpl ApiLocal;
        
        public Func<string, TaskCompletionSource<string>> LocalMethodInvoked;

        public Endpoint(string name, AbstractTransport transport, ICodec codec)
        {
            var rmiSrv = new TransportRmi(transport, codec.WrapLogging(CreateLogger(name)))
            {
                Logger = CreateLogger("rmi-" + name)
            };
            ApiRemote = rmiSrv.CreateRemote<ITestApi>();
            ApiLocal = new TestApiImpl();
            rmiSrv.RegisterLocal<ITestApi>(ApiLocal);
            ApiLocal.QueryNestedDelegate = ApiRemote;
            LocalMethodInvoked = methodName => CreateTaskCompletionSource(
                ApiLocal.Events, TestApiEvent.Inv, methodName);
        }
    }

    private Endpoint _srv;
    
    private Endpoint _cln;
    
    protected override void ConfigureLoggingBuilder(ILoggingBuilder builder)
    {
        AddTransportRmiHtmlLogger(builder);
    }

    [OneTimeSetUp]
    public new void OneTimeSetUp()
    {
        var (transportCln, transportSrv) = LocalTransport.CreatePair();

        var codec = new JsonCodec()
            .AddConverter(MethodCallJsonConverter.Instance)
            .AddConverter(ExceptionJsonConverter.Instance);

        _srv = new Endpoint("srv", transportSrv, codec);
        _cln = new Endpoint("cln", transportCln, codec);
    }
    
    [TearDown]
    public void Teardown()
    {
    }
    
    [Test]
    public async Task QuerySimpleBeanAsyncThrows()
    {
        await TestNotImplementedAsync(() => _cln.ApiRemote.QuerySimpleBeanAsyncThrows("123", 321));
    }
    
    [Test]
    public async Task QueryStringAsynce()
    {
        var invoked = _srv.LocalMethodInvoked(nameof(ITestApi.QueryStringAsync));
        var result = await _cln.ApiRemote.QueryStringAsync();
        Assert.That(result, Is.EqualTo(nameof(ITestApi.QueryStringAsync)));
        await AwaitResult(invoked);
    }

    [Test]
    public void QuerySimpleBean()
    {
        var result = _cln.ApiRemote.QuerySimpleBean("123", 321);
        Assert.That(result.ValueString, Is.EqualTo("123"));
        Assert.That(result.ValueInt, Is.EqualTo(321));
    }

    [Test]
    public async Task QuerySimpleBeanAsync()
    {
        var result = await _cln.ApiRemote.QuerySimpleBeanAsync("123", 321);
        Assert.That(result.ValueString, Is.EqualTo("123"));
        Assert.That(result.ValueInt, Is.EqualTo(321));
    }

    [Test]
    public void QuerySimpleBeanThrows()
    {
        TestNotImplemented(() => _cln.ApiRemote.QuerySimpleBeanThrows("123", 321));
    }

    [Test]
    public void QueryThrows()
    {
        TestNotImplemented(() => _cln.ApiRemote.QueryThrows());
    }

    [Test]
    public async Task CallVoid()
    {
        var invoked = _srv.LocalMethodInvoked(nameof(ITestApi.CallVoid));
        _cln.ApiRemote.CallVoid();
        await AwaitResult(invoked);
    }

    [Test]
    public async Task QueryString()
    {
        var invoked = _srv.LocalMethodInvoked(nameof(ITestApi.QueryString));
        var result = _cln.ApiRemote.QueryString();
        Assert.That(result, Is.EqualTo(nameof(ITestApi.QueryString)));
        await AwaitResult(invoked);
    }
    
    [Test]
    public async Task QueryNested()
    {
        var invoked = _srv.LocalMethodInvoked(nameof(ITestApi.QueryNested));
        var result = _cln.ApiRemote.QueryNested(1);
        Assert.That(result, Is.EqualTo(0));
        await AwaitResult(invoked);
    }
}
