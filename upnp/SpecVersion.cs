using System.Xml;
using utils.Logging;

namespace Upnp
{
    class SpecVersion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1823:AvoidUnusedPrivateFields"
        )]
        string major;

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1823:AvoidUnusedPrivateFields"
        )]
        string minor;

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
