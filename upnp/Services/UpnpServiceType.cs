﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace upnp.Services
{
    class UpnpServiceType : IUpnpServiceType
    {
        public string ServiceTypeUri
        {
            get { return _serviceTypeUri; }
        }

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
        public string VendorDomainName
        {
            get { return _serviceTypeUri.Split(':')[1]; }
        }

        internal UpnpServiceType(string serviceTypeUri)
        {
            this._serviceTypeUri = serviceTypeUri;
        }

        readonly string _serviceTypeUri;
    }
}
