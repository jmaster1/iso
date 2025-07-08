using IsoNet.Iso.Common;

namespace IsoNet.Iso.Client
{
    internal class IsoClientApi : IIsoClientApi
    {
        private readonly IsoClient _client;

        public IsoClientApi(IsoClient client)
        {
            _client = client;
        }

        public void WorldStarted() => _client.WorldStarted();

        public void WorldFrameReport(int frame) => _client.WorldFrameReport(frame);
    }
}
