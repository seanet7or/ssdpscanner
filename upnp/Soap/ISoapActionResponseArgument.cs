using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace upnp.Soap
{
    interface ISoapActionResponseArgument
    {
        string Name { get; }

        string Value { get; }

        UInt32 ParseUi4();
    }
}
