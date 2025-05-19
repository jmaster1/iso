namespace IsoNet.Iso.Common;

public interface IIsoServerApi
{
    string CreateWorld();
    
    void JoinWorld(string worldId);
}
