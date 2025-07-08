using Common.IO.Transport.Rmi;

namespace Iso.Net.Common
{
    [Call]
    public interface IIsoWorldApi
    {
        void Build(string id, int x, int y, bool flip = false);
    }
}
