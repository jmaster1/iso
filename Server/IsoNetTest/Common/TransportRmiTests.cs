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
    }
    
    private ITestApi _apiClnRemote;
    
    private ITestApi _apiSrvRemote;
    
    private TestApiImpl _apiSrvLocal;
    
    private TestApiImpl _apiClnLocal;
    
    private Func<string, TaskCompletionSource<string>> _serverMethodInvoked;
    
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

        var rmiSrv = new TransportRmi(transportSrv, codec.WrapLogging(CreateLogger("srv")))
        {
            Logger = CreateLogger("rmiSrv")
        };
        _apiSrvRemote = rmiSrv.CreateRemote<ITestApi>();
        _apiSrvLocal = new TestApiImpl();
        rmiSrv.RegisterLocal<ITestApi>(_apiSrvLocal);

        var rmiCln = new TransportRmi(transportCln, codec.WrapLogging(CreateLogger("cln")))
        {
            Logger = CreateLogger("rmiCln"),
            RequestIdOffset = 1000
        };
        _apiClnRemote = rmiCln.CreateRemote<ITestApi>();
        _apiClnLocal = new TestApiImpl();
        rmiCln.RegisterLocal<ITestApi>(_apiClnLocal);

        _serverMethodInvoked = name => CreateTaskCompletionSource(_apiSrvLocal.Events, TestApiEvent.Inv, name);
    }
    
    [TearDown]
    public void Teardown()
    {
    }
    
    [Test]
    public async Task QuerySimpleBeanAsyncThrows()
    {
        await TestNotImplementedAsync(() => _apiClnRemote.QuerySimpleBeanAsyncThrows("123", 321));
    }

    [Test]
    public async Task QueryStringAsynce()
    {
        var invoked = _serverMethodInvoked(nameof(ITestApi.QueryStringAsync));
        var result = await _apiClnRemote.QueryStringAsync();
        Assert.That(result, Is.EqualTo(nameof(ITestApi.QueryStringAsync)));
        await AwaitResult(invoked);
    }

    [Test]
    public void QuerySimpleBeans()
    {
        var result = _apiClnRemote.QuerySimpleBean("123", 321);
        Assert.That(result.ValueString, Is.EqualTo("123"));
        Assert.That(result.ValueInt, Is.EqualTo(321));
    }

    [Test]
    public async Task QuerySimpleBeanAsync()
    {
        var result = await _apiClnRemote.QuerySimpleBeanAsync("123", 321);
        Assert.That(result.ValueString, Is.EqualTo("123"));
        Assert.That(result.ValueInt, Is.EqualTo(321));
    }

    [Test]
    public void QuerySimpleBeanThrows()
    {
        TestNotImplemented(() => _apiClnRemote.QuerySimpleBeanThrows("123", 321));
    }

    [Test]
    public void QueryThrows()
    {
        TestNotImplemented(() => _apiClnRemote.QueryThrows());
    }

    [Test]
    public async Task CallVoid()
    {
        var invoked = _serverMethodInvoked(nameof(ITestApi.CallVoid));
        _apiClnRemote.CallVoid();
        await AwaitResult(invoked);
    }

    [Test]
    public async Task QueryString()
    {
        var invoked = _serverMethodInvoked(nameof(ITestApi.QueryString));
        var result = _apiClnRemote.QueryString();
        Assert.That(result, Is.EqualTo(nameof(ITestApi.QueryString)));
        await AwaitResult(invoked);
    }
}
