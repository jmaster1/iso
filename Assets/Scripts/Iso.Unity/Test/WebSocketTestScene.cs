using System.Threading.Tasks;
using UnityEngine;

namespace Iso.Unity.Test
{
    public class WebSocketTestScene : MonoBehaviour
    {
        WebSocketClient client = new();
        
        private async Task Awake()
        {
            client._log = line => Debug.Log("Client: " + line);
            await client.Connect("ws://localhost:7000/ws/", msg => {});
            await client.SendMessage("Hello World");
            await client.SendMessage("Hello World2");
            // Thread.Sleep(1000);
            //
            // await client.Disconnect();
        }
        
        private void Update()
        {
            
        }
    }
}
