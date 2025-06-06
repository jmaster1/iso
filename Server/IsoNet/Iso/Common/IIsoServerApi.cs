using IsoNet.Core.Transport.Rmi;

namespace IsoNet.Iso.Common;

public interface IIsoServerApi
{
    string CreateWorld(int width, int height);

    WorldInfo JoinWorld(string worldId);
    
    [Call]
    void StartWorld();
}
