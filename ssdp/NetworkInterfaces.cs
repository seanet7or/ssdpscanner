using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Ssdp
{
    public class NetworkInterfaces
    {
        public IEnumerable<IPAddress> GetConnectedIPAddresses()
        {
            var addresses = new List<IPAddress>();

            /*
            
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    addresses.Add(ip);
                }
            }*/

            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (
                    item.OperationalStatus == OperationalStatus.Up
                    && item.NetworkInterfaceType != NetworkInterfaceType.Loopback
                )
                {
                    foreach (
                        UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses
                    )
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            addresses.Add(ip.Address);
                        }
                    }
                }
            }
            return addresses;
        }
    }
}
