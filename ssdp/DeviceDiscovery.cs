using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Text;
using ssdp;

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

        List<SsdpClient>? clients = [];

        public EventHandler<DeviceDiscoveredEventArgs>? DeviceDiscovered { get; set; }

        readonly ConcurrentDictionary<string, MSearchResponse> searchResponses = new();
        bool disposed;

        public DeviceDiscovery(IEnumerable<IPAddress> ipAddressesToSendDiscoveryRequest)
        {
            ArgumentNullException.ThrowIfNull(ipAddressesToSendDiscoveryRequest);
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
                        client.Send(
                            searchRequestData,
                            searchRequestData.Length,
                            new IPEndPoint(
                                IPAddress.Parse(Defines.MulticastIpv4Address),
                                Defines.SsdpPort
                            )
                        );
                        await Task.Delay(SearchRequestsInterval);
                    }
                }
            }
            // Wait 2 extra seconds if a delayed answer is received
            await Task.Delay((searchRequest.MaxWaitTimeInSecs + 2) * 1000);
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
                        try
                        {
                            var searchResponse = new MSearchResponse(receiveString);
                            if (searchResponse.SearchTarget != null && remoteEndpoint != null)
                            {
                                if (
                                    searchResponses.TryAdd(
                                        searchResponse.SearchTarget,
                                        searchResponse
                                    )
                                )
                                {
                                    DeviceDiscovered?.Invoke(
                                        this,
                                        new DeviceDiscoveredEventArgs(
                                            searchResponse,
                                            remoteEndpoint
                                        )
                                    );
                                }
                            }
                        }
                        catch (SsdpException ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Unexpected response: " + receiveString);
                    }
                }
            }
            // ObjectDisposedException or SocketException may occur if a late answer is received, but the socket is already closed. We can ignore these errors.
            catch (ObjectDisposedException) { }
            catch (System.Net.Sockets.SocketException) { }
        }
    }
}
