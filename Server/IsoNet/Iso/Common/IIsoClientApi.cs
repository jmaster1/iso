using IsoNet.Core.Transport.Rmi;

namespace IsoNet.Iso.Common;

public interface IIsoClientApi
{
    [Call]
    void WorldStarted();
}
