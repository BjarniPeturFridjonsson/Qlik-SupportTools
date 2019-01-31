using System.Net;

namespace Eir.Common.Net
{
    public static class SecurityInitiator
    {
        public static void InitProtocols()
        {
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Ssl3 |
                SecurityProtocolType.Tls |
                SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls12;
        }
    }
}