using System.Net;
using System.Net.Sockets;

namespace Ssdp
{
    internal class SsdpClient : UdpClient
    {
        public SsdpClient(IPAddress iPAddress)
        {
            Client = new SsdpSocket(iPAddress);
        }

        public IAsyncResult BeginReceive(Action<IAsyncResult> receiveCallback, SsdpClient client)
        {
            return base.BeginReceive((ar) => receiveCallback(ar), client);
        }

        public bool Connected => Client != null;
    }
}
