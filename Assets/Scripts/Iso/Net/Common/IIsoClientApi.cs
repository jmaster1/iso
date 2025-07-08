using Common.IO.Transport.Rmi;

namespace Iso.Net.Common
{
    public interface IIsoClientApi
    {
        [Call]
        void WorldStarted();
    
        [Call]
        void WorldFrameReport(int frame);
    }
}
