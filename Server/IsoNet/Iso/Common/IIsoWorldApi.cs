using IsoNet.Core.Transport.Rmi;

namespace IsoNet.Iso.Common;

[Call]
public interface IIsoWorldApi
{
    void Build(string id, int x, int y, bool flip = false);
}
