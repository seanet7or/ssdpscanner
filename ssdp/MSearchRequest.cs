using System.Text;

namespace Ssdp
{
    public class MSearchRequest
    {
        readonly string header;

        internal MSearchRequest(string searchTarget, int maxWaitTimeInSecs)
        {
            MaxWaitTimeInSecs = maxWaitTimeInSecs;
            header = $"""
                      M-SEARCH * HTTP/1.1
                      HOST: 239.255.255.250:1900
                      MAN: "ssdp:discover"
                      MX: {maxWaitTimeInSecs}
                      ST: {searchTarget}
                      CPFN.UPNP.ORG: UpnpExplorer

                      """;
            // MX: REQUIRED. Field value contains maximum wait time in seconds. MUST be greater than or equal to 1 and SHOULD be less than 5 inclusive.
        }

        public int MaxWaitTimeInSecs { get; private set; }

        internal byte[] GetHeaderData()
        {
            return Encoding.ASCII.GetBytes(header);
        }
    }
}
