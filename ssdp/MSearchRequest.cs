using System.Text;

namespace Ssdp
{
    public class MSearchRequest
    {
        string header;

        internal MSearchRequest(string searchTarget, int maxWaitTimeInSecs)
        {
            MaxWaitTimeInSecs = maxWaitTimeInSecs;
            header =
                "M-SEARCH * HTTP/1.1\r\nHOST: 239.255.255.250:1900\r\nMAN: \"ssdp:discover\"\r\nMX: "
                + maxWaitTimeInSecs
                + "\r\nST: "
                + searchTarget
                + "\r\n\r\n";

            // REQUIRED. Field value contains maximum wait time in seconds. MUST be greater than or equal to 1 and SHOULD be less than 5 inclusive.
        }

        public int MaxWaitTimeInSecs { get; private set; }

        internal byte[] GetHeaderData()
        {
            return Encoding.ASCII.GetBytes(header);
        }
    }
}
