using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace upnp.Devices
{
    class UpnpDeviceType : IUpnpDeviceType
    {
        readonly string deviceTypeUri;

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
        public string VendorDomainName
        {
            get { return deviceTypeUri.Split(':')[1]; }
        }

        public UpnpDeviceType(string deviceTypeUri)
        {
            this.deviceTypeUri = deviceTypeUri;
        }
    }
}
