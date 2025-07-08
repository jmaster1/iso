using System;
using System.Threading.Tasks;
using Common.IO.Streams;
using Common.Unity.Boot;
using Common.Util.Http;
using Iso.Net.Client;
using Iso.Player;
using UnityEngine;

namespace Iso.Unity.Test
{
    public class PlayerNetTestScene : MonoBehaviour, IHttpQueryProcessor
    {
        public IsoWorld World = new();
        
        private IsoClient _client;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Unicom.Debug.HttpRouter.AddHandler(this);
        }

        // Update is called once per frame
        void Update()
        {
            //throw new Exception("??");
            if (Time.frameCount % 100 == 0)
            {
                Connect(null);
            }
        }

        public void OnHttpRequest(HttpQuery query)
        {
        }

        public void OnHttpResponse(HttpQuery query, HtmlWriter html)
        {
        }

        [HttpInvoke]
        public async Task Connect(string url)
        {
            
                _client = await IsoClient.CreateWebsocket(World, Unicom.GameTime);
            
            
        }
    }
}
