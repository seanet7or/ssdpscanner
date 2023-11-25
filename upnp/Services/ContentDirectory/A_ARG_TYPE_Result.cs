using System.Xml;
using utils.Logging;

namespace upnp.Services.ContentDirectory
{
    public class ArgTypeResult : List<UpnpObject>
    {
        public ArgTypeResult(string xml)
        {
            using var reader = XmlReader.Create(new StringReader(xml));
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    if (reader.Name == "xml")
                    {
                        reader.Read();
                    }
                    else if (reader.Name == "DIDL-Lite")
                    {
                        while (reader.Read())
                        {
                            if (reader.IsStartElement())
                            {
                                if (reader.Name == "container")
                                {
                                    Add(new UpnpContainer(reader));
                                }
                                else if (reader.Name == "item")
                                {
                                    Add(new UpnpItem(reader));
                                }
                                else
                                {
                                    Log.LogWarning(
                                        "Unexpected node {0} in DIDL-Lite node",
                                        reader.Name
                                    );
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
                    else
                    {
                        Log.LogWarning("Unexpected node name in ArgTypeResult: " + reader.Name);
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
