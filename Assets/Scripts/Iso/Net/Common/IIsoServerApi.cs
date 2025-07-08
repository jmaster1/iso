using Common.IO.Transport.Rmi;

namespace Iso.Net.Common
{
    public interface IIsoServerApi
    {
        [Call]
        void ClientId(string id);
    
        string CreateWorld(int width, int height);

        WorldInfo JoinWorld(string worldId);
    
        [Call]
        void StartWorld();
    }
}
