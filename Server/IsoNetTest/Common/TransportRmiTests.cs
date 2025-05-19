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
            return new SimpleBean
            {
                ValueString = stringValue,
                ValueInt = intValue
            };
        }
    }
    
    [Test]
    public async Task Test()
    {
        var (transportCln, transportSrv) = LocalTransport.CreatePair();
        var codec = new JsonCodec()
            .AddConverter(MethodCallJsonConverter.Instance)
            .AddConverter(new ExceptionJsonConverter());
        
        //
        // server
        var rmiSrv = new TransportRmi(transportSrv, codec.WrapLogging(CreateLogger("srv")));
        var apiSrv = new TestApiImpl();
        rmiSrv.RegisterLocal<ITestApi>(apiSrv);
        TaskCompletionSource<string> ServerMethodInvoked(string name) => 
            CreateTaskCompletionSource(apiSrv.Events, TestApiEvent.Inv, name);
        
        //
        // client
        var rmiCln = new TransportRmi(transportCln, codec.WrapLogging(CreateLogger("cln")));
        var apiCln = rmiCln.CreateRemote<ITestApi>();
        
                
        //
        // QueryStringAsync
        var queryStringAsyncInvoked = ServerMethodInvoked(nameof(ITestApi.QueryStringAsync));
        var queryStringAsyncResult = await apiCln.QueryStringAsync();
        Assert.That(queryStringAsyncResult, Is.EqualTo(nameof(ITestApi.QueryStringAsync)));
        await AwaitResult(queryStringAsyncInvoked);
        
        //
        // QuerySimpleBean
        var simpleBean = apiCln.QuerySimpleBean("123", 321);
        Assert.That(simpleBean.ValueString, Is.EqualTo("123"));
        Assert.That(simpleBean.ValueInt, Is.EqualTo(321));
        
        
        //
        // QueryThrows
        var queryThrowsInvoked = ServerMethodInvoked(nameof(ITestApi.QueryThrows));
        try
        {
            apiCln.QueryThrows();
            Assert.Fail();
        }
        catch (NotImplementedException)
        {
        }
        await AwaitResult(queryThrowsInvoked);

        //
        // CallVoid
        var callVoidInvoked = ServerMethodInvoked(nameof(ITestApi.CallVoid));
        apiCln.CallVoid();
        await AwaitResult(callVoidInvoked);
        
        //
        // QueryString
        var queryStringInvoked = ServerMethodInvoked(nameof(ITestApi.QueryString));
        var queryStringResult = apiCln.QueryString();
        Assert.That(queryStringResult, Is.EqualTo(nameof(ITestApi.QueryString)));
        await AwaitResult(queryStringInvoked);

    }
}
