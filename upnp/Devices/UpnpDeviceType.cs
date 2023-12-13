namespace upnp.Devices
{
    class UpnpDeviceType(string deviceTypeUri) : IUpnpDeviceType
    {
        readonly string deviceTypeUri = deviceTypeUri;

        public StandardDeviceType StandardDeviceType
        {
            get
            {
                if (VendorDomainName == "schemas-upnp-org")
                {
                    var type = deviceTypeUri.Split(':')[3];
                    if (type == "MediaRenderer")
                    {
                        return StandardDeviceType.MediaRenderer;
                    }
                    if (type == "MediaServer")
                    {
                        return StandardDeviceType.MediaServer;
                    }
                }
                return StandardDeviceType.None;
            }
        }

        // schemas-upnp-org für Standardgeräte
        public string VendorDomainName => deviceTypeUri.Split(':')[1];
    }
}
