namespace IsoNet.Iso.Common;

public interface IIsoServerApi
{
    WorldInfo CreateWorld(int width, int height);

    void StartWorld();
    
    void JoinWorld(string worldId);
}
