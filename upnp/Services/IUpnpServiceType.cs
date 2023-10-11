namespace upnp.Services
{
    public interface IUpnpServiceType
    {
        string VendorDomainName { get; }

        StandardServiceType StandardServiceType { get; }

        string ServiceTypeUri { get; }
    }
}
