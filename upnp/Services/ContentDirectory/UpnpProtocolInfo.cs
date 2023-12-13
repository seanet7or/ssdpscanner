namespace upnp.Services.ContentDirectory
{
    // A string that identifies the recommended HTTP protocol for transmitting the resource (see also UPnP A/V Conection Manager Service
    // template, section 2.5.2). If not present, then the content has not yet been fully imported by CDS and is not yet accesible for
    // playback purposes.
    // <protocol>’:’ <network>’:’<contentFormat>’:’<additionalInfo>
    public class UpnpProtocolInfo(string value)
    {
        readonly string rawValue = value;

        // Mime-Type bei HttpGet
        public string ContentFormat => rawValue.Split(':')[2];

        public UpnpProtocol Protocol
        {
            get
            {
                switch (rawValue.Split(':')[0])
                {
                    case "*":
                    case "http-get":
                        return UpnpProtocol.HttpGet;
                    case "rtsp-rtp-udp":
                        return UpnpProtocol.RtspRtpUdp;
                    case "internal":
                        return UpnpProtocol.Internal;
                    case "iec61883":
                        return UpnpProtocol.Iec61883;
                    default:
                        // «registered ICANN domain	name of	vendor »
                        return UpnpProtocol.Custom;
                }
            }
        }
    }
}
