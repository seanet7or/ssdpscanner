using System.Xml;

namespace upnp.Services
{
    class Argument
    {
#pragma warning disable IDE0052 // Remove unread private members
        readonly string? name;

        // REQUIRED. Defines whether argument is an input or output parameter. MUST be either “in” or “out” and not both.
        // All input arguments MUST be listed before any output arguments.
        readonly string? direction;

        // OPTIONAL. Identifies at most one output argument as the return value. If included, MUST be included as a
        // subelement of the first output argument. (Element only; no value.)
        readonly bool retVal;

        // REQUIRED. MUST be the name of a state variable. Case Sensitive. Defines the type of the argument;
        // see further explanation below in this section.
        readonly string? relatedStateVariable;
#pragma warning restore IDE0052 // Remove unread private members

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
