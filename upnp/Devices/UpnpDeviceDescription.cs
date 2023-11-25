using System.Xml;
using Ssdp;
using Upnp;
using upnp.Services;
using upnp.Services.ContentDirectory;
using utils.Logging;

namespace upnp.Devices
{
    public class UpnpDeviceDescription
    {
        readonly MSearchResponse searchResponse;

        internal SpecVersion? SpecVersion { get; private set; }

        public IUpnpDevice? Device
        {
            get { return _device; }
        }
        UpnpDevice? _device;

        public UpnpDeviceDescription(MSearchResponse searchResponse, IHttpClient httpClient)
        {
            this.searchResponse = searchResponse;
            this.httpClient = httpClient;
        }

        readonly IHttpClient httpClient;

        static string? LanguageCode()
        {
            /*var locale = Java.Util.Locale.Default;
            if (locale != null)
            {
                var res = locale.Language;
                if (!string.IsNullOrEmpty(res))
                {
                    if (!string.IsNullOrEmpty(locale.Country))
                    {
                        res = res + "-" + locale.Country;
                        return res;
                    }
                    return res;
                }
            }*/
            return null;
        }

        public async Task<bool> ReadDescriptionAsync()
        {
            try
            {
                if (searchResponse.Location == null)
                {
                    Log.LogWarning("Can not read UPNP device description: Location is null");
                    return false;
                }
                var langCode = LanguageCode();
                var xmlStream = await httpClient.GetAsync(langCode, searchResponse.Location);
                var xmlReader = XmlReader.Create(xmlStream);

                while (xmlReader.Read())
                {
                    if (xmlReader.IsStartElement())
                    {
                        if (xmlReader.IsEmptyElement)
                        {
                            //Debug.WriteLine("<{0}/>", xmlReader.Name);
                        }
                        else
                        {
                            if (xmlReader.Name == "root")
                            {
                                ParseRootNode(xmlReader);
                            }
                        }
                    }
                }
                await ReadServiceDescsAsync();
                return true;
            }
            catch (Exception e)
            {
                Log.LogWarning("Errror reading UPNP device description: " + e);
                return false;
            }
        }

        async Task ReadServiceDescsAsync()
        {
            if (_device?.DeviceServiceList != null)
            {
                foreach (var service in _device.DeviceServiceList)
                {
                    var serviceDescription = new UpnpServiceDescription(service, httpClient);
                    if (await serviceDescription.ReadDescriptionAsync())
                    {
                        if (
                            service.ServiceType?.StandardServiceType
                            == StandardServiceType.ContentDirectory
                        )
                        {
                            var contentDirectory = new UpnpContentDirectory(
                                serviceDescription,
                                httpClient
                            );
                            _device.AddService(contentDirectory);
                            await contentDirectory.GetSearchCapabilitiesAsync();
                        }
                    }
                }
            }
        }

        void ParseRootNode(XmlReader reader)
        {
            string? urlBase = null;
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "specVersion":
                            SpecVersion = new SpecVersion(reader);
                            break;
                        case "URLBase":
                            reader.Read();
                            urlBase = reader.ReadContentAsString();
                            break;
                        case "device":
                            _device = new UpnpDevice(reader);
                            break;
                        default:
                            reader.Read();
                            reader.ReadContentAsString();
                            break;
                    }
                }
                else
                {
                    break;
                }
            }

            if (string.IsNullOrEmpty(urlBase))
            {
                urlBase = searchResponse.Location;
            }
            if (_device != null && !string.IsNullOrEmpty(urlBase))
            {
                _device.BaseUrl = new Url(urlBase);
            }
        }
        /*
        void ParseRootNode(XmlNode root)
        {
            string urlBase = null;
            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.Name == "specVersion")
                {
                    SpecVersion = new SpecVersion(node);
                }
                else if (node.Name == "URLBase")
                {
                    urlBase = node.InnerText;
                }
                else if (node.Name == "device")
                {
                    _device = new UpnpDevice(node);
                }
                else
                {
                    Console.WriteLine("Unexpected Node in root node: " + node.Name);
                }
            }

            if (string.IsNullOrEmpty(urlBase))
            {
                urlBase = searchResponse.UpnpDescriptionLink;
            }
            _device.BaseUrl = new Url(urlBase);
        }*/
    }
}
