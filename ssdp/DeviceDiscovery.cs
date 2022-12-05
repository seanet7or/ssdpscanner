using System.Collections.Concurrent;
using System.Net;
using System.Text;

namespace Ssdp
{
    public class DeviceDiscovery : IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                if (disposing)
                {
                    if (clients != null)
                    {
                        foreach (var udpClient in clients)
                        {
                            udpClient.Close();
                        }
                        clients.Clear();
                        clients = null;
                    }
                }
            }
        }

        const int RequestsToSend = 8;
        const int SearchRequestsInterval = 20;

        List<SsdpClient>? clients = new List<SsdpClient>();

        public EventHandler<DeviceDiscoveredEventArgs>? DeviceDiscovered { get; set; }

        readonly ConcurrentDictionary<string, MSearchResponse> searchResponses
            = new ConcurrentDictionary<string, MSearchResponse>();
        bool disposed;

        public DeviceDiscovery(IEnumerable<IPAddress> ipAddressesToSendDiscoveryRequest)
        {
            if (ipAddressesToSendDiscoveryRequest == null)
            {
                throw new ArgumentNullException("ipAddressesToSendDiscoveryRequest");
            }
            foreach (var address in ipAddressesToSendDiscoveryRequest)
            {
                clients.Add(new SsdpClient(address));
            }
        }

        public async Task SearchAsync(int secondsToSearch)
        {
            var searchRequest = new MSearchRequest(SearchTargets.AllSsdp, secondsToSearch);
            await SearchAsync(searchRequest);
        }

        public async Task SearchAsync(MSearchRequest searchRequest)
        {
            var searchRequestData = searchRequest.GetHeaderData();

            if (clients != null)
            {
                foreach (var client in clients)
                {
                    client.BeginReceive(ReceiveCallback, client);
                    for (int i = 0; i < RequestsToSend; i++)
                    {
                        client.Send(searchRequestData,
                            searchRequestData.Length,
                            new IPEndPoint(IPAddress.Parse(Defines.MulticastIpv4Address), Defines.SsdpPort));
                        await Task.Delay(SearchRequestsInterval);
                    }
                }
            }
            await Task.Delay((searchRequest.MaxWaitTimeInSecs + 1) * 1000);
        }

        void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var remoteEndpoint = new IPEndPoint(IPAddress.Any, Defines.SsdpPort);
                var client = (SsdpClient?)ar.AsyncState;
                if (client != null && client.Connected)
                {
                    var data = client.EndReceive(ar, ref remoteEndpoint);
                    string receiveString = Encoding.ASCII.GetString(data, 0, data.Length);
                    client.BeginReceive(ReceiveCallback, client);

                    if (receiveString.StartsWith("HTTP/1.1 200 OK", StringComparison.Ordinal))
                    {
                        var searchResponse = new MSearchResponse(receiveString);
                        if (searchResponse.SearchTarget != null && remoteEndpoint != null)
                        {
                            if (searchResponses.TryAdd(searchResponse.SearchTarget, searchResponse))
                            {
                                if (DeviceDiscovered != null)
                                {
                                    DeviceDiscovered(this, new DeviceDiscoveredEventArgs(searchResponse, remoteEndpoint));
                                }
                            }
                        }
                    }
                }
            }
            catch (ObjectDisposedException)
            {

            }
            //Console.WriteLine("Received from {1}:\n{0}", receiveString, remoteEndpoint.ToString());
        }
    }
}
