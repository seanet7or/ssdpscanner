namespace upnp.Services
{
    class UpnpServiceType : IUpnpServiceType
    {
        public string ServiceTypeUri => _serviceTypeUri;

        public StandardServiceType StandardServiceType
        {
            get
            {
                if (VendorDomainName == "schemas-upnp-org")
                {
                    var type = _serviceTypeUri.Split(':')[3];
                    if (type == "ContentDirectory")
                    {
                        return StandardServiceType.ContentDirectory;
                    }
                }
                return StandardServiceType.None;
            }
        }

        // schemas-upnp-org für Standardgeräte
        public string VendorDomainName => _serviceTypeUri.Split(':')[1];

        internal UpnpServiceType(string serviceTypeUri)
        {
            this._serviceTypeUri = serviceTypeUri;
        }

        readonly string _serviceTypeUri;
    }
}
