using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace upnp.Soap
{
    class SoapActionResponseArgument : ISoapActionResponseArgument
    {
        public string? Name { get; internal set; }

        public string? Value { get; internal set; }

        public UInt32 ParseUi4()
        {
            UInt32 result;
            if (UInt32.TryParse(Value, out result))
            {
                return result;
            }
            return 0;
        }
    }
}
