using Iso.Net.Common;

namespace Iso.Net.Client
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
