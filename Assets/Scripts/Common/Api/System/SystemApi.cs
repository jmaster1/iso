using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Common.Api.System
{
    public class SystemApi : AbstractApi
    {
        /// <summary>
        /// Returns true if device is capable of connecting to internet
        /// </summary>
        public virtual bool IsNetworkConnected()
        {
            return true;
        }
        
        /// <summary>
        /// system language name retrieval
        /// </summary>
        public virtual string GetLanguage()
        {
            return null;
        }
        
        /// <summary>
        /// system platform name retrieval
        /// </summary>
        public virtual string GetPlatform()
        {
            return null;
        }
        
        public static string GetLocalIP()
        {
            return Dns.GetHostEntry(Dns.GetHostName())
                .AddressList.First(
                    f => f.AddressFamily == AddressFamily.InterNetwork)
                .ToString();
        }  
    }
}