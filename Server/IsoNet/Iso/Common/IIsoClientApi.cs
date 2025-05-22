using IsoNet.Core.Transport.Rmi;

namespace IsoNet.Iso.Common;

public interface IIsoClientApi
{
    [Call]
    void WorldСreated(WorldInfo info);
    
    [Call]
    void WorldStarted();
}
