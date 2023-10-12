using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace upnp.Services
{
    class AllowedValueRange
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1823:AvoidUnusedPrivateFields"
        )]
        string? minimum;

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1823:AvoidUnusedPrivateFields"
        )]
        string? maximum;

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1823:AvoidUnusedPrivateFields"
        )]
        string? step;

        public AllowedValueRange(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "minimum":
                            reader.Read();
                            minimum = (reader.ReadContentAsString());
                            break;
                        case "maximum":
                            reader.Read();
                            maximum = (reader.ReadContentAsString());
                            break;
                        case "step":
                            reader.Read();
                            step = (reader.ReadContentAsString());
                            break;
                        default:
                            reader.Read();
                            reader.ReadContentAsString();
                            break;
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
