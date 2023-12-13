using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpnpExplorer.Models
{
    internal class PhysicalDevice(Device rootDevice)
    {
        readonly IEnumerable<Device> rootDevices = [rootDevice];

        public string Authority => rootDevices.First().Location.Authority ?? string.Empty;
    }
}
