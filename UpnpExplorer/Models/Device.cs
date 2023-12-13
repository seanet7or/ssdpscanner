using ssdp;

namespace UpnpExplorer.Models
{
    public class Device(Url location)
    {
        public string DisplayName { get; set; } = string.Empty;

        public Url Location { get; } = location;
    }
}
