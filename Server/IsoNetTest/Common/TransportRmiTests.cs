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
        string RequestMethod();
    }

    private class TestApiImpl : ITestApi
    {
        public void CallMethod()
        {
            
        }

        public string RequestMethod()
        {
            return "result";
        }
    }
    
    [Test]
    public void Test()
    {
        var (transportCln, transportSrv) = LocalTransport.CreatePair();
        
        var rmiSrv = new TransportRmi(transportSrv, MethodCallJsonConverter.Codec.WrapLogging(Logger), )
        
        
        var codec = MethodCallJsonConverter.Codec.WrapLogging(Logger);

        var apiImpl = new TestApiImpl();
        var invoker = new MethodInvoker();
        invoker.Register<ITestApi>(apiImpl);
        
        var (api, _) = Proxy.Create<ITestApi>(call =>
        {
            using var stream = new MemoryStream();
            codec.Write(call, stream);
            stream.Position = 0;
            var result = codec.Read(stream);
            return Task.FromResult(invoker.Invoke(result));
        });
        
        // api.Method1();
        // var calls = apiImpl.GetCalls(nameof(ITestApi.Method1))!;
        // Assert.That(calls, Has.Count.EqualTo(1));
        // var args = calls[0].Args!;
        // Assert.That(args.Length, Is.EqualTo(0));
        //
        // api.Method2(123, "123", true, 'c');
        // calls = apiImpl.GetCalls(nameof(ITestApi.Method2))!;
        // Assert.That(calls, Has.Count.EqualTo(1));
        // args = calls[0].Args!;
        // Assert.That(args.Length, Is.EqualTo(4));
        // Assert.That(args[0], Is.EqualTo(123));
        // Assert.That(args[1], Is.EqualTo("123"));
        // Assert.That(args[2], Is.EqualTo(true));
        // Assert.That(args[3], Is.EqualTo('c'));
    }
}
