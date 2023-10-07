using System.Net;

namespace Ssdp
{
    class Defines
    {
        internal const string MulticastIpv4Address = "239.255.255.250";

        internal const int SsdpPort = 1900;

        internal static IPEndPoint SsdpMulticastEndpoint = new IPEndPoint(
            IPAddress.Parse(MulticastIpv4Address),
            SsdpPort
        );
    }
}
