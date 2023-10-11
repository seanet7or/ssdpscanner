using upnp.Services;
using upnp.Services.ContentDirectory;

namespace upnp.Devices
{
    public interface IUpnpDevice
    {
        Icon FindIcon(int requestedWidth);

        string FriendlyName { get; }

        IUpnpDeviceType DeviceType { get; }

        string SerialNumber { get; }

        string Manufacturer { get; }

        Guid Id { get; }

        string GetModelDescription();

        string ManufacturerUrl { get; }

        IEnumerable<UpnpService> Services { get; }

        string ModelNumber { get; }

        string ModelUrl { get; }

        string PresentationUrl { get; }

        IconList IconList { get; }

        IContentDirectory ContentDirectory { get; }

        Url BaseUrl { get; }
    }
}
