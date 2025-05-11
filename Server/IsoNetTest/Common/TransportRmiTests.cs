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
        void CallMethod();

        [Query]
        Task<string> RequestMethod();
    }
    
    private enum TestApiEvent
    {
        CallMethodInv,
        RequestMethodInv
    }

    private class TestApiImpl : ITestApi
    {
        public Events<TestApiEvent, ITestApi> Events = new();
        
        public void CallMethod()
        {
            Events.Fire(TestApiEvent.CallMethodInv, this);
        }

        public async Task<string> RequestMethod()
        {
            Events.Fire(TestApiEvent.RequestMethodInv, this);
            return "result";
        }
    }
    
    [Test]
    public async Task Test()
    {
        var (transportCln, transportSrv) = LocalTransport.CreatePair();
        var codec = new JsonCodec2().AddConverter(MethodCallJsonConverter.Instance);
        
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
        // var callMethodInvoked = CreateTaskCompletionSource(apiSrv.Events, TestApiEvent.CallMethodInv);
        // apiCln.CallMethod();
        // await AwaitResult(callMethodInvoked);
        // Assert.That(transportCln.MessageCountSent, Is.EqualTo(1));
        // Assert.That(transportCln.MessageCountReceived, Is.EqualTo(0));
        // Assert.That(transportSrv.MessageCountSent, Is.EqualTo(0));
        // Assert.That(transportSrv.MessageCountReceived, Is.EqualTo(1));
        
        //
        // RequestMethod
        var requestMethodInvoked = CreateTaskCompletionSource(apiSrv.Events, TestApiEvent.RequestMethodInv);
        var response = await apiCln.RequestMethod();
        Assert.That(response, Is.EqualTo("result"));
        await AwaitResult(requestMethodInvoked);
    }
}
