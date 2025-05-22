using IsoNet.Core.Transport.Rmi;

namespace IsoNet.Iso.Common;

public interface IIsoServerApi
{
    WorldInfo CreateWorld(int width, int height);

    [Call]
    void StartWorld();
    
    void JoinWorld(string worldId);
}
