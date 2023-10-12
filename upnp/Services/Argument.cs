using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace upnp.Services
{
    class Argument
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1823:AvoidUnusedPrivateFields"
        )]
        readonly string? name;

        // REQUIRED. Defines whether argument is an input or output parameter. MUST be either “in” or “out” and not both.
        // All input arguments MUST be listed before any output arguments.
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1823:AvoidUnusedPrivateFields"
        )]
        readonly string? direction;

        // OPTIONAL. Identifies at most one output argument as the return value. If included, MUST be included as a
        // subelement of the first output argument. (Element only; no value.)
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1823:AvoidUnusedPrivateFields"
        )]
        readonly bool retVal;

        // REQUIRED. MUST be the name of a state variable. Case Sensitive. Defines the type of the argument;
        // see further explanation below in this section.
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1823:AvoidUnusedPrivateFields"
        )]
        readonly string? relatedStateVariable;

        public Argument(XmlReader reader)
        {
            retVal = false;
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "name":
                            reader.Read();
                            name = reader.ReadContentAsString();
                            break;
                        case "direction":
                            reader.Read();
                            direction = reader.ReadContentAsString();
                            break;
                        case "retval":
                            reader.Read();
                            retVal = true;
                            break;
                        case "relatedStateVariable":
                            reader.Read();
                            relatedStateVariable = reader.ReadContentAsString();
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
