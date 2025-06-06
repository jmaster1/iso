using IsoNet.Iso.Common;

namespace IsoNet.Iso.Client;

internal class IsoClientApi(IsoClient client) : IIsoClientApi
{
    public void WorldStarted() => client.WorldStarted();

    public void WorldFrameReport(int frame) => client.WorldFrameReport(frame);
}
