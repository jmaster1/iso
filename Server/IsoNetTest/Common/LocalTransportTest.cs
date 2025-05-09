using IsoNet.Core.IO.Codec;
using IsoNet.Core.Transport;
using IsoNetTest.Core;

namespace IsoNetTest.Common;

public class LocalTransportTest : AbstractTests
{
        
    [Test]
    public async Task Test()
    {
        var (transportA, transportB) = LocalTransport.CreatePair();
        var (messengerA, tcsA) = CreateMessenger(transportA);
        var (messengerB, tcsB) = CreateMessenger(transportB);
        const string messageA = "hello from A";
        messengerA.SendMessage(messageA);
        const string messageB = "hello from B";
        messengerB.SendMessage(messageB);
        await CheckResult(tcsA, messageB);
        await CheckResult(tcsB, messageA);
    }

    private (CodecMessenger<string>, TaskCompletionSource<string>) CreateMessenger(LocalTransport transport)
    {
        var codec = new StringCodec();
        var tcs = new TaskCompletionSource<string>();
        var messenger = new CodecMessenger<string>(transport, codec, message =>
        {
            tcs.SetResult(message);
        }).Init();
        return (messenger, tcs);
    }

    private static async Task CheckResult(TaskCompletionSource<string> tcs, string message)
    {
        var result = await AwaitResult(tcs);
        Assert.That(result, Is.EqualTo(message));
    }
}
