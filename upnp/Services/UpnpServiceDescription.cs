using System.Xml;
using ssdp;
using Upnp;
using utils.Logging;

namespace upnp.Services
{
    public class UpnpServiceDescription(IUpnpDeviceService service, IHttpClient httpClient)
    {
        // REQUIRED. In service templates, defines the lowest version of the architecture on which the service can be implemented.
        // In actual UPnP services, defines the architecture on which the service is implemented.
        internal SpecVersion? SpecVersion { get; private set; }

        internal ActionList? ActionList { get; private set; }

        internal Url? AbsoluteServiceControlUrl => deviceService?.AbsoluteServiceControlUrl;

        internal string? ServiceTypeUri => deviceService?.ServiceTypeUri;

        internal ServiceStateTable? ServiceStateTable { get; private set; }

        readonly IUpnpDeviceService deviceService = service;

        readonly IHttpClient httpClient = httpClient;

        internal async Task<bool> ReadDescriptionAsync()
        {
            try
            {
                if (deviceService.AbsoluteServiceDescriptionUrl == null)
                {
                    Log.LogWarning("Can not read service description: URL is null");
                    return false;
                }
                var xmlStream = await httpClient.GetAsync(
                    null,
                    deviceService.AbsoluteServiceDescriptionUrl.ToString()
                );
                var xmlReader = XmlReader.Create(xmlStream);

                while (xmlReader.Read())
                {
                    if (xmlReader.IsStartElement())
                    {
                        if (xmlReader.Name == "xml")
                        {
                            xmlReader.Read();
                        }
                        else if (xmlReader.Name == "scpd")
                        {
                            while (xmlReader.Read())
                            {
                                if (xmlReader.IsStartElement())
                                {
                                    switch (xmlReader.Name)
                                    {
                                        case "specVersion":
                                            SpecVersion = new SpecVersion(xmlReader);
                                            break;
                                        case "actionList":
                                            ActionList = new ActionList(xmlReader);
                                            break;
                                        case "serviceStateTable":
                                            ServiceStateTable = new ServiceStateTable(xmlReader);
                                            break;
                                        default:
                                            Log.LogWarning(
                                                "Unexpected node name in scpd node: "
                                                    + xmlReader.Name
                                            );
                                            xmlReader.Read();
                                            xmlReader.ReadContentAsString();
                                            break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Log.LogWarning(
                                "Unexpected node name in service desc: " + xmlReader.Name
                            );
                            xmlReader.Read();
                            xmlReader.ReadContentAsString();
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.LogWarning("Errror reading UPNP service description: " + e.Message);
                return false;
            }
            return true;
        }
    }
}
