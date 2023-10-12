using System.Xml;
using utils.Logging;

namespace Upnp
{
    class SpecVersion
    {
#pragma warning disable IDE0052 // Remove unread private members
        readonly string? major;
        readonly string? minor;
#pragma warning restore IDE0052 // Remove unread private members

        internal SpecVersion(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    if (reader.Name == "major")
                    {
                        reader.Read();
                        major = reader.ReadContentAsString();
                    }
                    else if (reader.Name == "minor")
                    {
                        reader.Read();
                        minor = reader.ReadContentAsString();
                    }
                    else
                    {
                        Log.LogWarning("Unexpected Node in SpecVersion: " + reader.Name);
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
