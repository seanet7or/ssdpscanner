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

        internal static IPAddress GetLocalIPv4()
        {
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties adapterProperties = item.GetIPProperties();

                    if (adapterProperties.GatewayAddresses.FirstOrDefault() != null)
                    {
                        foreach (
                            UnicastIPAddressInformation ip in adapterProperties.UnicastAddresses
                        )
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                return ip.Address;
                            }
                        }
                    }
                }
            }
            return IPAddress.Any;
        }

        internal SsdpSocket(IPAddress address)
            : base(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, SsdpTtl);
            SetSocketOption(
                SocketOptionLevel.IP,
                SocketOptionName.AddMembership,
                new MulticastOption(IPAddress.Parse(Defines.MulticastIpv4Address), address)
            );

            Bind(new IPEndPoint(address, 0));
        }
    }
}
