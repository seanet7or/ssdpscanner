using System.Xml;
using utils.Logging;

namespace upnp.Services
{
    public class ServiceStateTable : Dictionary<string, StateVariable>
    {
        public ServiceStateTable(XmlReader reader)
        {
            if (reader != null)
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.Name == "stateVariable")
                        {
                            var stateVar = new StateVariable(reader);
                            Add(stateVar.Name, stateVar);
                        }
                        else
                        {
                            Log.LogWarning(
                                "Unexpected node {0} in service state table",
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
        }
    }
}
