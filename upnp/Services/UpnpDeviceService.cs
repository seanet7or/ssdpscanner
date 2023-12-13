using System.Xml;
using ssdp;
using upnp.Devices;
using utils.Logging;

namespace upnp.Services
{
    class UpnpDeviceService : IUpnpDeviceService
    {
        public string? ServiceTypeUri => ServiceType?.ServiceTypeUri;

        public Url? AbsoluteServiceDescriptionUrl =>
            _scpdUrl != null ? device.BaseUrl?.ResolveRelativeToThisBaseUrl(_scpdUrl) : null;

        // Required. URL for service description (nee Service Control Protocol Definition URL).
        // (cf. section below on service description.) May be relative to base URL. Specified by UPnP vendor. Single URL.
        readonly Url? _scpdUrl;

        // Required. URL for eventing (cf. section on Eventing). May be relative to base URL.
        // Must be unique within the device; no two services may have the same URL for eventing.
        // If the service has no evented variables, it should not have eventing (cf. section on Eventing);
        // if the service does not have eventing, this element must be present but should be empty, i.e., <eventSubURL></eventSubURL>.
        // Specified by UPnP vendor. Single URL.
        public string? EventSubURL { get; private set; }

        public Url? AbsoluteServiceControlUrl =>
            (_controlUrl != null)
                ? device.BaseUrl?.ResolveRelativeToThisBaseUrl(_controlUrl)
                : null;

        // Required. URL for control (cf. section on Control). May be relative to base URL. Specified by UPnP vendor. Single URL.
        readonly Url? _controlUrl;

        // REQUIRED. Service identifier. MUST be unique within this device description. Single URI.
        public string? ServiceId { get; private set; }

        // Required. UPnP service type. MUST NOT contain a hash character (#, 23 Hex in UTF-8). Single URI.
        public IUpnpServiceType? ServiceType { get; private set; }

        readonly UpnpDevice device;

        public UpnpDeviceService(XmlReader reader, UpnpDevice device)
        {
            this.device = device;
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "serviceType":
                            reader.Read();
                            ServiceType = new UpnpServiceType(reader.ReadContentAsString());
                            break;
                        case "serviceId":
                            reader.Read();
                            ServiceId = reader.ReadContentAsString();
                            break;
                        case "SCPDURL":
                            reader.Read();
                            _scpdUrl = new Url(reader.ReadContentAsString());
                            break;
                        case "controlURL":
                            reader.Read();
                            _controlUrl = new Url(reader.ReadContentAsString());
                            break;
                        case "eventSubURL":
                            reader.Read();
                            EventSubURL = reader.ReadContentAsString();
                            break;
                        default:
                            if (reader.IsEmptyElement)
                            {
                                reader.Read();
                            }
                            else
                            {
                                Log.LogWarning("Unexpected Node in service node: " + reader.Name);
                                reader.Read();
                                reader.ReadContentAsString();
                            }
                            break;
                    }
                }
                else
                {
                    return;
                }
            }
        }
    }
}
