using Common.IO.Streams;
using Common.Unity.Boot;
using Common.Util.Http;
using Iso.Net.Client;
using UnityEngine;

namespace Iso.Unity.Test
{
    public class PlayerNetTestScene : MonoBehaviour, IHttpQueryProcessor
    {
        private IsoClient _client = new();
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Unicom.Debug.HttpRouter.AddHandler(this);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void OnHttpRequest(HttpQuery query)
        {
        }

        public void OnHttpResponse(HttpQuery query, HtmlWriter html)
        {
        }

        [HttpInvoke]
        public void Connect(string url)
        {
            
        }
    }
}
