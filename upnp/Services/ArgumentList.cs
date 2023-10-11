using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using utils.Logging;

namespace upnp.Services
{
    class ArgumentList : List<Argument>
    {
        public ArgumentList(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    if (reader.Name == "argument")
                    {
                        Add(new Argument(reader));
                    }
                    else
                    {
                        Log.LogWarning(
                            "Unexpected node name in argument list node: " + reader.Name
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
    }
}
