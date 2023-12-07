using System.Xml;
using utils.Logging;

namespace upnp.Services.ContentDirectory
{
    public class UpnpItem : UpnpObject
    {
        public UpnpItem(XmlReader reader)
        {
            if (reader != null)
            {
                ReadObjectAttributes(reader);

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (ReadObjectNode(reader)) { }
                        else
                        {
                            Log.LogWarning("Unexpected Node in item node: " + reader.Name);
                            reader.Read();
                            reader.ReadContentAsString();
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}
