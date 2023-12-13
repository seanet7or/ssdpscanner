using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Ssdp
{
    class SsdpSocket : Socket
    {
        // Upnp 1.1: Note: The TTL for the IP packet SHOULD default to 2 and SHOULD be configurable.
        // Upnp 1.0: TTL SHOULD default to 4
        const int SsdpTtl = 4;

        internal SsdpSocket(IPAddress address)
            : base(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, SsdpTtl);
            Bind(new IPEndPoint(address, 0));
        }
    }
}
