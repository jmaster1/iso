using Common.Lang.Observable;
using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using IsoNet.Core.Transport;
using IsoNet.Core.Transport.Rmi;
using IsoNetTest.Core;

namespace IsoNetTest.Common;

public class TransportRmiTests : AbstractTests
{
    private interface ITestApi
    {
        [Call]
        void CallVoid();

        [Query]
        Task<string> QueryStringAsync();
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
    }
    
    [Test]
    public async Task Test()
    {
        var (transportCln, transportSrv) = LocalTransport.CreatePair();
        var codec = new JsonCodec().AddConverter(MethodCallJsonConverter.Instance);
        
        //
        // server
        var rmiSrv = new TransportRmi(transportSrv, codec.WrapLogging(CreateLogger("srv")));
        var apiSrv = new TestApiImpl();
        rmiSrv.RegisterLocal<ITestApi>(apiSrv);
        
        //
        // client
        var rmiCln = new TransportRmi(transportCln, codec.WrapLogging(CreateLogger("cln")));
        var apiCln = rmiCln.CreateRemote<ITestApi>();
        
        //
        // CallMethod
        var callMethodInvoked = CreateTaskCompletionSource(
            apiSrv.Events, TestApiEvent.Inv, nameof(ITestApi.CallVoid));
        apiCln.CallVoid();
        await AwaitResult(callMethodInvoked);
        Assert.That(transportCln.MessageCountSent, Is.EqualTo(1));
        Assert.That(transportCln.MessageCountReceived, Is.EqualTo(0));
        Assert.That(transportSrv.MessageCountSent, Is.EqualTo(0));
        Assert.That(transportSrv.MessageCountReceived, Is.EqualTo(1));
        
        //
        // RequestMethod
        var requestMethodInvoked = CreateTaskCompletionSource(
            apiSrv.Events, TestApiEvent.Inv, nameof(ITestApi.QueryStringAsync));
        var response = await apiCln.QueryStringAsync();
        Assert.That(response, Is.EqualTo(nameof(ITestApi.QueryStringAsync)));
        await AwaitResult(requestMethodInvoked);
    }
}
