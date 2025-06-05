using Common.TimeNS;
using IsoNet.Core.Proxy;

namespace IsoNet.Iso.Common;

public static class IsoCommon
{
    public static readonly TimeSpan Delta = TimeSpan.FromMilliseconds(20);
    
    public const string AttrFrame = "frame";

    public static int GetFrame(this MethodCall call)
    {
        return call.GetAttr<int>(AttrFrame, Time.FrameUndefined);
    }
    
    public static void SetFrame(this MethodCall call, int frame)
    {
        call.SetAttr(AttrFrame, frame);
    }
}