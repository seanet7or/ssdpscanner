namespace upnp.Devices
{
    public interface IUpnpDeviceType
    {
        string VendorDomainName { get; }

        StandardDeviceType StandardDeviceType { get; }
    }
}
