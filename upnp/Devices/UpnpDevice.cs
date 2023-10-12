using System.Xml;
using upnp.Services;
using upnp.Services.ContentDirectory;
using utils.Logging;

namespace upnp.Devices
{
    class UpnpDevice : IUpnpDevice
    {
        readonly object iconListLock = new object();

        public Icon? FindIcon(int requestedWidth)
        {
            lock (iconListLock)
            {
                IconList?.Sort();
                if (IconList != null)
                {
                    foreach (var icon in IconList)
                    {
                        if (requestedWidth < icon.Width)
                        {
                            return icon;
                        }
                    }
                }
                return IconList?.Last();
            }
        }

        public IContentDirectory? ContentDirectory
        {
            get { return _services?.OfType<IContentDirectory>().FirstOrDefault(); }
        }

        public string? GetModelDescription()
        {
            if (!string.IsNullOrEmpty(modelDescription))
            {
                return modelDescription;
            }
            return modelName;
        }

        // Recommended. Long description for end user. Should be localized (cf. ACCEPT-LANGUAGE and CONTENT-LANGUAGE headers).
        // Specified by UPnP vendor. String. Should be < 128 characters.
        readonly string? modelDescription;

        // Required. Model name. May be localized (cf. ACCEPT-LANGUAGE and CONTENT-LANGUAGE headers).
        // Specified by UPnP vendor. String. Should be < 32 characters.
        readonly string? modelName;

        // Recommended. Serial number. May be localized (cf. ACCEPT-LANGUAGE and CONTENT-LANGUAGE headers).
        // Specified by UPnP vendor. String. Should be < 64 characters.
        public string? SerialNumber { get; private set; }

        // REQUIRED. UPnP device type. Single URI.
        public IUpnpDeviceType? DeviceType { get; private set; }

        // Required. Short description for end user. Should be localized (cf. ACCEPT-LANGUAGE and CONTENT-LANGUAGE headers).
        // Specified by UPnP vendor. String. Should be < 64 characters.
        public string? FriendlyName { get; private set; }

        // Required. Manufacturer's name. May be localized (cf. ACCEPT-LANGUAGE and CONTENT-LANGUAGE headers).
        // Specified by UPnP vendor. String. Should be < 64 characters.
        public string? Manufacturer { get; private set; }

        public Guid Id { get; private set; }

        // Optional. Web site for Manufacturer. May be localized (cf. ACCEPT-LANGUAGE and CONTENT-LANGUAGE headers).
        // May be relative to base URL. Specified by UPnP vendor. Single URL.
        public string? ManufacturerUrl { get; private set; }

        // Recommended. Model number. May be localized (cf. ACCEPT-LANGUAGE and CONTENT-LANGUAGE headers).
        // Specified by UPnP vendor. String. Should be < 32 characters.
        public string? ModelNumber { get; private set; }

        // Optional. Web site for model. May be localized (cf. ACCEPT-LANGUAGE and CONTENT-LANGUAGE headers).
        // May be relative to base URL. Specified by UPnP vendor. Single URL.
        public string? ModelUrl { get; private set; }

        // Recommended. URL to presentation for device (cf. section on Presentation). May be relative to base URL.
        // Specified by UPnP vendor. Single URL.
        public string? PresentationUrl { get; private set; }

        // Required if and only if device has one or more icons. Specified by UPnP vendor.
        public IconList? IconList { get; private set; }

        public IEnumerable<UpnpService> Services
        {
            get { return _services; }
        }

        readonly List<UpnpService> _services = new List<UpnpService>();

        internal void AddService(UpnpService service)
        {
            _services.Add(service);
        }

        // Optional.
        internal DeviceServiceList? DeviceServiceList { get; private set; }

        public Url? BaseUrl { get; internal set; }

        internal UpnpDevice(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    if (reader.Name == "deviceType")
                    {
                        reader.Read();
                        DeviceType = new UpnpDeviceType(reader.ReadContentAsString());
                    }
                    else if (reader.Name == "friendlyName")
                    {
                        reader.Read();
                        FriendlyName = reader.ReadContentAsString();
                    }
                    else if (reader.Name == "manufacturer")
                    {
                        reader.Read();
                        Manufacturer = reader.ReadContentAsString();
                    }
                    else if (reader.Name == "UDN")
                    {
                        // Required. Unique Device Name. Universally-unique identifier for the device, whether root or embedded.
                        // Must be the same over time for a specific device instance (i.e., must survive reboots). Must match the value of the
                        // NT header in device discovery messages. Must match the prefix of the USN header in all discovery messages.
                        // (The section on Discovery explains the NT and USN headers.)
                        // Must begin with uuid: followed by a UUID suffix specified by a UPnP vendor. Single URI.
                        reader.Read();
                        var uuid = reader.ReadContentAsString().Split(':')[1];
                        Id = Guid.Parse(uuid);
                    }
                    else if (reader.Name == "manufacturerURL")
                    {
                        reader.Read();
                        ManufacturerUrl = reader.ReadContentAsString();
                    }
                    else if (reader.Name == "modelDescription")
                    {
                        reader.Read();
                        modelDescription = reader.ReadContentAsString();
                    }
                    else if (reader.Name == "modelName")
                    {
                        reader.Read();
                        modelName = reader.ReadContentAsString();
                    }
                    else if (reader.Name == "modelNumber")
                    {
                        reader.Read();
                        ModelNumber = reader.ReadContentAsString();
                    }
                    else if (reader.Name == "modelURL")
                    {
                        reader.Read();
                        ModelUrl = reader.ReadContentAsString();
                    }
                    else if (reader.Name == "presentationURL")
                    {
                        reader.Read();
                        PresentationUrl = reader.ReadContentAsString();
                    }
                    else if (reader.Name == "iconList")
                    {
                        lock (iconListLock)
                        {
                            IconList = new IconList(reader, this);
                        }
                    }
                    else if (reader.Name == "serialNumber")
                    {
                        reader.Read();
                        SerialNumber = reader.ReadContentAsString();
                    }
                    else if (reader.Name == "serviceList")
                    {
                        DeviceServiceList = new DeviceServiceList(reader, this);
                    }
                    else
                    {
                        Log.LogWarning("Unexpected Node in Device node: " + reader.Name);
                        reader.Read();
                        reader.ReadContentAsString();
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
