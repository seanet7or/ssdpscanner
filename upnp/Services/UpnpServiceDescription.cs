using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Upnp;
using utils.Logging;

namespace upnp.Services
{
    public class UpnpServiceDescription
    {
        // REQUIRED. In service templates, defines the lowest version of the architecture on which the service can be implemented.
        // In actual UPnP services, defines the architecture on which the service is implemented.
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode"
        )]
        internal SpecVersion SpecVersion { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode"
        )]
        internal ActionList ActionList { get; private set; }

        internal Url AbsoluteServiceControlUrl
        {
            get { return deviceService.AbsoluteServiceControlUrl; }
        }

        internal string ServiceTypeUri
        {
            get { return deviceService.ServiceTypeUri; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode"
        )]
        internal ServiceStateTable ServiceStateTable { get; private set; }

        readonly IUpnpDeviceService deviceService;

        readonly IHttpClient httpClient;

        public UpnpServiceDescription(IUpnpDeviceService service, IHttpClient httpClient)
        {
            this.httpClient = httpClient;
            deviceService = service;
        }

        internal async Task<bool> ReadDescriptionAsync()
        {
            try
            {
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
            }
            return true;
        }
    }
}
