using System.Net;

namespace Ssdp
{
    public class DeviceDiscoveredEventArgs(MSearchResponse searchResponse, IPEndPoint endPoint)
    {
        public MSearchResponse SearchResponse { get; set; } = searchResponse;

        public IPEndPoint EndPoint { get; set; } = endPoint;
    }
}
