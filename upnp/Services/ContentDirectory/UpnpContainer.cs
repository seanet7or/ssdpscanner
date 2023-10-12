using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using utils.Logging;

namespace upnp.Services.ContentDirectory
{
    public class UpnpContainer : UpnpObject
    {
        public IEnumerable<string> SearchClasses
        {
            get { return _searchClasses; }
        }

        readonly List<string> _searchClasses = new List<string>();

        // Child count for the object. Applies to containers only.
        public int ChildCount { get; private set; }

        // When true, the ability to perform a Search action under a container is enabled, otherwise a Search under that container will
        // return no results. The default value of this attribute when if it is absent on a container is false.
        public bool Searchable
        {
            get { return _searchable; }
        }

        readonly bool _searchable = false;

        public UpnpContainer(XmlReader reader)
        {
            if (reader != null)
            {
                ReadObjectAttributes(reader);
                var childCountAttr = reader.GetAttribute("childCount");
                if (!string.IsNullOrEmpty(childCountAttr))
                {
                    ChildCount = int.Parse(childCountAttr, CultureInfo.InvariantCulture);
                }
                var searchableAttr = reader.GetAttribute("searchable");
                if (!string.IsNullOrEmpty(searchableAttr))
                {
                    _searchable = InnerTextToBoolean(searchableAttr);
                }

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (ReadObjectNode(reader)) { }
                        else if (reader.Name == "upnp:searchClass")
                        {
                            reader.Read();
                            _searchClasses.Add(reader.ReadContentAsString());
                        }
                        else
                        {
                            Log.LogWarning(
                                "Unexpected Node in upnp container node: " + reader.Name
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
