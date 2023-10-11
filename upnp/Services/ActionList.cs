using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using utils.Logging;

namespace upnp.Services
{
    class ActionList : List<UpnpServiceAction>
    {
        /*
        public ActionList(XmlNode actionListNode)
        {
            foreach (XmlNode node in actionListNode.ChildNodes)
            {
                if (node.Name == "action")
                {
                    Add(new UpnpServiceAction(node));
                }
                else
                {
                    Console.WriteLine("Unexpected node name in action list node: " + node.Name);
                }
            }
        }*/

        public ActionList(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    if (reader.Name == "action")
                    {
                        Add(new UpnpServiceAction(reader));
                    }
                    else
                    {
                        Log.LogWarning("Unexpected node name in action list node: " + reader.Name);
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
