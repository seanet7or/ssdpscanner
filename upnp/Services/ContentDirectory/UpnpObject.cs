using System.Xml;

namespace upnp.Services.ContentDirectory
{
    public class UpnpObject
    {
        public string? BestItemImage(int widthInPixels, int heightInPixels)
        {
            var resUrl = BestResourceImage(widthInPixels, heightInPixels);
            if (string.IsNullOrEmpty(resUrl))
            {
                return AlbumArtUri;
            }
            return resUrl;
        }

        string? BestResourceImage(int widthInPixels, int heightInPixels)
        {
            if (_resourceFiles.Count > 0)
            {
                var httpGetRes = _resourceFiles
                    .Where(r => r.ProtocolInfo?.Protocol == UpnpProtocol.HttpGet)
                    .ToList();
                if (httpGetRes.Count > 0)
                {
                    var mimeTypeRes = httpGetRes
                        .Where(
                            r =>
                                (r.ProtocolInfo?.ContentFormat == "image/jpeg")
                                || (r.ProtocolInfo?.ContentFormat == "image/png")
                        )
                        .ToList();
                    if (mimeTypeRes.Count > 0)
                    {
                        mimeTypeRes.Sort();
                        foreach (var res in mimeTypeRes)
                        {
                            if (widthInPixels < res.HorizontalResolution)
                            {
                                if (heightInPixels < res.VerticalResolution)
                                {
                                    return res.Uri;
                                }
                            }
                        }
                        return mimeTypeRes.Last().Uri;
                    }
                    return null;
                }
                return null;
            }
            return null;
        }

        public IList<UpnpResource> ResourceFiles
        {
            get { return _resourceFiles; }
        }

        readonly List<UpnpResource> _resourceFiles = new();

        public IList<string>? Rights
        {
            get { return _rights; }
        }

        readonly List<string> _rights = new();

        // General-purpose tag where a user can annotate an object with some user-specific information
        public string? UserAnnotation { get; private set; }

        public string? Title { get; private set; }

        // Title of the album to which the item belongs.
        public string? Album { get; private set; }

        protected static bool InnerTextToBoolean(string text)
        {
            if (text == "1")
            {
                return true;
            }
            return false;
        }

        public string? Id { get; protected set; }

        // When true, ability to modify a given object is confined to the Content Directory. Control point metadata write access is disabled.
        public bool Restricted { get; protected set; }

        // id property of object’s parent. The parentID of the Content Directory ‘root’ container must be set to the reserved value of “-1”.
        // No other parentID attribute of any other Content Directory object may take this value.
        public string? ParentId { get; private set; }

        // Class of the object
        public string? Class { get; private set; }

        // When present, controls the modifiability of the resources of a given object. Ability of a Control Point
        // to change writeStatus of a given resource(s) is implementation dependent. Allowed values are: WRITABLE,
        // PROTECTED, NOT_WRITABLE, UNKNOWN, MIXED.
        public string? WriteStatus { get; private set; }

        // Reference to album art. Values must be properly escaped URIs as described in [RFC 2396].
        public string? AlbumArtUri { get; private set; }

        public string? Desc { get; private set; }

        protected void ReadObjectAttributes(XmlReader reader)
        {
            Id = reader.GetAttribute("id");
            var restrictedAttr = reader.GetAttribute("restricted");
            if (!string.IsNullOrEmpty(restrictedAttr))
            {
                Restricted = InnerTextToBoolean(restrictedAttr);
            }
            ParentId = reader.GetAttribute("parentID");
        }

        // ISO 8601, of the form "YYYY-MM-DD"
        public string? Date { get; private set; }

        protected bool ReadObjectNode(XmlReader reader)
        {
            // title, Dublin Core, string, Name of the object
            switch (reader.Name)
            {
                case "dc:rights":
                    reader.Read();
                    _rights.Add(reader.ReadContentAsString());
                    return true;
                case "dc:date":
                    reader.Read();
                    Date = reader.ReadContentAsString();
                    return true;
                case "desc":
                    reader.Read();
                    Desc = reader.ReadContentAsString();
                    return true;
                case "dc:title":
                    reader.Read();
                    Title = reader.ReadContentAsString();
                    return true;
                case "upnp:class":
                    reader.Read();
                    Class = reader.ReadContentAsString();
                    return true;
                case "upnp:writeStatus":
                    reader.Read();
                    WriteStatus = reader.ReadContentAsString();
                    return true;
                case "upnp:userAnnotation":
                    reader.Read();
                    UserAnnotation = reader.ReadContentAsString();
                    return true;
                case "upnp:albumArtURI":
                    reader.Read();
                    AlbumArtUri = reader.ReadContentAsString();
                    return true;
                case "res":
                    _resourceFiles.Add(new UpnpResource(reader));
                    return true;
                case "upnp:album":
                    reader.Read();
                    Album = reader.ReadContentAsString();
                    return true;
                default:
                    return false;
            }
        }
    }
}
