using System.Xml;
using upnp.Devices;
using utils.Logging;

namespace upnp.Services
{
    class DeviceServiceList : List<IUpnpDeviceService>
    {
        public DeviceServiceList(XmlReader reader, UpnpDevice device)
        {
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    if (reader.Name == "service")
                    {
                        Add(new UpnpDeviceService(reader, device));
                    }
                    else
                    {
                        Log.LogWarning("Unexpected Node in servicelist node: " + reader.Name);
                        reader.Read();
                        reader.ReadContentAsString();
                        reader.ReadEndElement();
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
