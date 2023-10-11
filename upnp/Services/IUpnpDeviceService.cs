namespace upnp.Services
{
    public interface IUpnpDeviceService
    {
        string ServiceTypeUri { get; }

        IUpnpServiceType ServiceType { get; }

        Url AbsoluteServiceDescriptionUrl { get; }

        Url AbsoluteServiceControlUrl { get; }
    }
}
