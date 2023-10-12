using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using utils.Logging;

namespace upnp.Services
{
    class UpnpServiceAction
    {
        internal string? Name { get; private set; }

        internal ArgumentList? ArgumentList { get; private set; }

        public UpnpServiceAction(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    if (reader.Name == "name")
                    {
                        reader.Read();
                        Name = reader.ReadContentAsString();
                    }
                    else if (reader.Name == "argumentList")
                    {
                        ArgumentList = new ArgumentList(reader);
                    }
                    else
                    {
                        Log.LogWarning("Unexpected node name in action node: " + reader.Name);
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
