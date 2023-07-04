using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Common.Unity.Boot;

namespace Common.Unity.Util.EventTracker
{
    public class EventSender
    {
        const string Header = "{\"records\":[{\"value\":\"";
        const string Footer = "\"}]}";
        
        HttpClient client;

        public async Task<HttpResponseMessage> Send(string file, EventTrackerInfo info)
        {
            if (client == null)
            {
                client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", info.AuthKey);
                client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.kafka.v2+json");
            }
            client.Timeout = TimeSpan.FromSeconds(info.SendTimeout);
            using (FileStream fileStream = File.OpenRead(file))
            using (ZipRemoteStream zipStream = new ZipRemoteStream(fileStream, Header, Footer))
            {
                using (var content = new StreamContent(zipStream))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.kafka.json.v2+json");
                    string url = Unicom.IsDebug ? info.UrlDev : info.UrlProd;
                    return await client.PostAsync(url, content);
                }
            }
        }
    }
}
