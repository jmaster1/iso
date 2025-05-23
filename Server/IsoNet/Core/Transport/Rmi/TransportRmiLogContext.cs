using Common.Util;
using Microsoft.Extensions.Logging;

namespace IsoNet.Core.Transport.Rmi;

public class TransportRmiLogContext
{
    private static readonly AsyncLocal<RmiLogContext?> Current = new();

    public static RmiLogContext GetCurrent()
    {
        return Current.Value ?? default;
    }

    public static EventId GetCurrentEvent() => GetCurrent().EventId;
    
    public static IDisposable Push(MessageType messageType, int requestId, string eventName)
    {
        var eventId = new EventId(requestId, eventName);
        var mc = new RmiLogContext(eventId, messageType);
        Current.Value = mc;
        return new DisposableAction(() => Current.Value = null);
    }
}

public readonly struct RmiLogContext(EventId eventId, MessageType messageType)
{
    public readonly EventId EventId = eventId;
    
    public readonly MessageType MessageType = messageType;
}
