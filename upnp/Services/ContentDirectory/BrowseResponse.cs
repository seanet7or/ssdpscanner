namespace upnp.Services.ContentDirectory
{
    public class BrowseResponse
    {
        public BrowseResponse(ArgTypeResult res)
        {
            _result = res;
        }

        ArgTypeResult _result;

        // This variable is used in conjunction with those actions that include a Result parameter.
        // The structure of the result is a DIDL-Lite XML fragment:
        // • Optional XML header e.g. <?xml version=”1.0” ?>
        // • <DIDL-Lite> is the root tag.
        // • <container> is the tag representing container objects.
        // • <item> is the tag representing item objects.
        // • Tags in the Dublin Core (dc) and UPnP (upnp) namespaces represent object metadata.
        public ArgTypeResult Result
        {
            get { return _result; }
        }

        // ContainerUpdateID (see Terms, sec. 2.5.21)of the container being described if a container is specified in ObjectID.
        // If the control point has an UpdateID for the container that is not equal to the UpdateID last returned, then the
        // control point should refresh all its state relative to that container. If the ObjectID is zero, then the UpdateID
        // returned is SystemUpdateID (see sec. 2.5.20).
        public ArgTypeUpdateId UpdateId { get; set; }

        // If BrowseMetadata is specified in the BrowseFlags then TotalMatches = 1, else if BrowseDirectChildren
        // is specified in the BrowseFlags then TotalMatches = total number of objects in the container specified
        // for the Browse() action (independent of the starting index specified by the StartingIndex argument).
        // If the CDS cannot compute TotalMatches and NumberReturned is not equal to zero, then TotalMatches = 0.
        // If the CDS cannot compute TotalMatches and NumberReturned is equal to zero, then the CDS should return an error code 720.
        public ArgTypeCount TotalMatches { get; set; }

        // Number of objects returned in this result. If BrowseMetadata is specified in the BrowseFlags, then NumberReturned = 1
        public ArgTypeCount NumberReturned { get; set; }
    }
}
