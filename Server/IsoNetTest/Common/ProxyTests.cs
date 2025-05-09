using IsoNet.Core.IO.Codec;
using IsoNet.Core.Proxy;
using IsoNetTest.Core;

namespace IsoNetTest.Common;

internal class MethodInv
{
    public object[]? Args;

    public MethodInv(object[]? args)
    {
        Args = args ?? [];
    }
}

public class ProxyTests : AbstractTests
{
    private interface ITestApi
    {
        void Method1();

        void Method2(int number, string str, bool flag, char c);
    }

    private class TestApiImpl : ITestApi
    {
        readonly Dictionary<string, List<MethodInv>> _calls = new();
        
        private void AddCall(string methodName, params object[]? args)
        {
            var call = new MethodInv(args);
            if (!_calls.TryGetValue(methodName, out var list))
            {
                _calls[methodName] = list = [];
            }

            list.Add(call);
        }
        
        public List<MethodInv>? GetCalls(string methodName)
        {
            return _calls.TryGetValue(methodName, out var list) ? list : null;
        }
        
        public void Method1()
        {
            AddCall(nameof(Method1));
        }

        public void Method2(int number, string str, bool flag, char c)
        {
            AddCall(nameof(Method2), number, str, flag, c);
        }
    }
    
    [Test]
    public void Test()
    {
        var codec = MethodCallJsonConverter.Codec.WrapLogging(Logger);

        var apiImpl = new TestApiImpl();
        var invoker = new MethodInvoker();
        invoker.Register<ITestApi>(apiImpl);
        
        var (api, _) = Proxy.Create<ITestApi>((async call =>
        {
            using var stream = new MemoryStream();
            codec.Write(call, stream);
            stream.Position = 0;
            var result = codec.Read(stream);
            return invoker.Invoke(result);
        }));
        
        api.Method1();
        var calls = apiImpl.GetCalls(nameof(ITestApi.Method1))!;
        Assert.That(calls, Has.Count.EqualTo(1));
        var args = calls[0].Args!;
        Assert.That(args.Length, Is.EqualTo(0));
        
        api.Method2(123, "123", true, 'c');
        calls = apiImpl.GetCalls(nameof(ITestApi.Method2))!;
        Assert.That(calls, Has.Count.EqualTo(1));
        args = calls[0].Args!;
        Assert.That(args.Length, Is.EqualTo(4));
        Assert.That(args[0], Is.EqualTo(123));
        Assert.That(args[1], Is.EqualTo("123"));
        Assert.That(args[2], Is.EqualTo(true));
        Assert.That(args[3], Is.EqualTo('c'));
    }
}
