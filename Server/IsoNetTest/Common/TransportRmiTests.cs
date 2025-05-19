using Common.Lang.Observable;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using IsoNet.Core.Transport;
using IsoNet.Core.Transport.Rmi;
using IsoNetTest.Core;

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
    
    private ITestApi apiCln;
    
    private TestApiImpl apiSrv;
    
    private Func<string, TaskCompletionSource<string>> ServerMethodInvoked;

    [SetUp]
    public void Setup()
    {
        var (transportCln, transportSrv) = LocalTransport.CreatePair();

        var codec = new JsonCodec()
            .AddConverter(MethodCallJsonConverter.Instance)
            .AddConverter(new ExceptionJsonConverter());

        var rmiSrv = new TransportRmi(transportSrv, codec.WrapLogging(CreateLogger("srv")));
        apiSrv = new TestApiImpl();
        rmiSrv.RegisterLocal<ITestApi>(apiSrv);

        var rmiCln = new TransportRmi(transportCln, codec.WrapLogging(CreateLogger("cln")));
        apiCln = rmiCln.CreateRemote<ITestApi>();

        ServerMethodInvoked = name => CreateTaskCompletionSource(apiSrv.Events, TestApiEvent.Inv, name);
    }
    
    [TearDown]
    public void Teardown()
    {
    }
    
    [Test]
    public async Task QuerySimpleBeanAsyncThrows_ThrowsNotImplemented()
    {
        await TestNotImplementedAsync(() => apiCln.QuerySimpleBeanAsyncThrows("123", 321));
    }

    [Test]
    public async Task QueryStringAsync_ReturnsExpectedValue()
    {
        var invoked = ServerMethodInvoked(nameof(ITestApi.QueryStringAsync));
        var result = await apiCln.QueryStringAsync();
        Assert.That(result, Is.EqualTo(nameof(ITestApi.QueryStringAsync)));
        await AwaitResult(invoked);
    }

    [Test]
    public void QuerySimpleBean_ReturnsCorrectValues()
    {
        var result = apiCln.QuerySimpleBean("123", 321);
        Assert.That(result.ValueString, Is.EqualTo("123"));
        Assert.That(result.ValueInt, Is.EqualTo(321));
    }

    [Test]
    public async Task QuerySimpleBeanAsync_ReturnsCorrectValues()
    {
        var result = await apiCln.QuerySimpleBeanAsync("123", 321);
        Assert.That(result.ValueString, Is.EqualTo("123"));
        Assert.That(result.ValueInt, Is.EqualTo(321));
    }

    [Test]
    public void QuerySimpleBeanThrows_ThrowsNotImplemented()
    {
        TestNotImplemented(() => apiCln.QuerySimpleBeanThrows("123", 321));
    }

    [Test]
    public void QueryThrows_ThrowsNotImplementedAndIsInvoked()
    {
        TestNotImplemented(() => apiCln.QueryThrows());
    }

    [Test]
    public async Task CallVoid_IsInvoked()
    {
        var invoked = ServerMethodInvoked(nameof(ITestApi.CallVoid));
        apiCln.CallVoid();
        await AwaitResult(invoked);
    }

    [Test]
    public async Task QueryString_ReturnsExpectedValueAndIsInvoked()
    {
        var invoked = ServerMethodInvoked(nameof(ITestApi.QueryString));
        var result = apiCln.QueryString();
        Assert.That(result, Is.EqualTo(nameof(ITestApi.QueryString)));
        await AwaitResult(invoked);
    }
}
