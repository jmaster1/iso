using IsoNet.Core.Transport.Rmi;

namespace IsoNet.Iso.Common;

public interface IIsoServerApi
{
    void CreateWorld(int width, int height);

    [Call]
    void StartWorld();
    
    void JoinWorld(string worldId);
}
