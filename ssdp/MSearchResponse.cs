using System.Diagnostics;
using ssdp;

namespace Ssdp
{
    public class MSearchResponse
    {
        // shall also be able to accept UUIDs that have not been formatted according to the rules specified below,
        // as formatting rules are not specified in UPnP 1.0 other than the requirement that a UUID is a string.
        // [ UPNP DA 2.0 p23 ]
        public string? Id { get; private set; }

        // Field value contains Search Target. Single URI, required
        public string? SearchTarget { get; private set; }

        // Description, Single absolute URL, required
        // URL to the UPnP description of the root device
        public string Location { get; private set; }

        // OS/version UPnP/2.0 product/version, required
        public string? Server { get; private set; }

        // uuid:device-UUID::urn:schemas-upnp-org:device:deviceType:ver
        // uuid:device-UUID::urn:schemas-upnp-org:service:serviceType:ver
        // uuid:device-UUID::urn:domain-name:device:deviceType:ver
        // uuid:device-UUID::urn:domain-name:service:serviceType:ver
        // Required. Field value contains Unique Service Name.
        public string Usn { get; private set; }

        public string? Header { get; private set; }

        public bool IsValid
        {
            get
            {
                return validResponseLine
                    && (secondsToCache >= 0)
                    && (!string.IsNullOrEmpty(Location))
                    && (!string.IsNullOrEmpty(Server))
                    && (!string.IsNullOrEmpty(SearchTarget))
                    && (!string.IsNullOrEmpty(Usn))
                    && (!string.IsNullOrEmpty(Id))
                    && (!Id.Equals(Guid.Empty));
            }
        }

        public MSearchResponse(string receivedHeader)
        {
            if (!string.IsNullOrEmpty(receivedHeader))
            {
                this.Header = receivedHeader;

                try
                {
                    foreach (
                        var line in receivedHeader.Split(
                            new[] { "\r\n" },
                            StringSplitOptions.RemoveEmptyEntries
                        )
                    )
                    {
                        if (line == "HTTP/1.1 200 OK")
                        {
                            validResponseLine = true;
                        }
                        else if (
                            line.StartsWith("CACHE-CONTROL:", StringComparison.OrdinalIgnoreCase)
                        )
                        {
                            var maxAge = line[14..].Trim();
                            var seconds = maxAge.Split('=').ElementAt(1).Trim();
                            if (!Int32.TryParse(seconds, out secondsToCache))
                            {
                                Debug.WriteLine("Error parsing CACHE-CONTROL header: " + line);
                                secondsToCache = -1;
                            }
                        }
                        else if (line.StartsWith("LOCATION:", StringComparison.OrdinalIgnoreCase))
                        {
                            Location = line[9..].Trim();
                        }
                        else if (line.StartsWith("SERVER:", StringComparison.OrdinalIgnoreCase))
                        {
                            Server = line[7..].Trim();
                        }
                        else if (line.StartsWith("ST:", StringComparison.OrdinalIgnoreCase))
                        {
                            SearchTarget = line[3..].Trim();
                        }
                        else if (line.StartsWith("USN:", StringComparison.OrdinalIgnoreCase))
                        {
                            Usn = line[4..].Trim();
                            Id = Usn.Split(':').ElementAt(1);
                        }
                        else if (line.StartsWith("EXT:", StringComparison.OrdinalIgnoreCase))
                        {
                            // ignore: Required for backwards compatibility with UPnP 1.0. (Header field name only; no field value.)
                        }
                        else
                        {
                            Debug.WriteLine("Unknown header line: " + line);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error parsing search response: " + e);
                }
            }
            if (Location == null)
            {
                throw new SsdpException(
                    "Location is missing in search response: " + receivedHeader
                );
            }
            if (Usn == null)
            {
                throw new SsdpException("USN is missing in search response: " + receivedHeader);
            }
        }

        readonly int secondsToCache = -1;
        readonly bool validResponseLine;
    }
}
