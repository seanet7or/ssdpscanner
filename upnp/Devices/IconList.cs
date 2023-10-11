using System.Xml;
using utils.Logging;

namespace upnp.Devices
{
    public class IconList : List<Icon>
    {
        /*
        internal IconList(XmlNode iconListNode, UpnpDevice device)
        {
            foreach (XmlNode node in iconListNode.ChildNodes)
            {
                if (node.Name == "icon")
                {
                    Add(new Icon(node, device));
                }
                else
                {
                    Console.WriteLine("Unexpected Node in iconlist node: " + node.Name);
                }
            }
        }*/

        internal IconList(XmlReader reader, UpnpDevice device)
        {
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    if (reader.Name == "icon")
                    {
                        Add(new Icon(reader, device));
                    }
                    else
                    {
                        Log.LogWarning("Unexpected Node in iconlist node: " + reader.Name);
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
