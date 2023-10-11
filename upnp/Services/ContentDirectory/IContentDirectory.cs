namespace upnp.Services.ContentDirectory
{
    public interface IContentDirectory
    {
        // Required
        // This action returns the searching capabilities that are supported by the device.
        Task GetSearchCapabilitiesAsync();

        // Required
        // Returns the CSV list of meta-data tags that can be used in sortCriteria
        Task GetSortCapabilitiesAsync();

        // This action returns the current value of state variable SystemUpdateID. It can be used by clients that want
        // to ‘poll’ for any changes in the Content Directory (as opposed to subscribing to events).
        Task<UInt32> GetSystemUpdateIdAsync();

        // This action allows the caller to incrementally browse the native hierarchy of the Content Directory objects
        // exposed by the Content Directory Service, including information listing the classes of objects available in
        // any particular object container.
        Task<BrowseResponse> BrowseAsync(
            ArgTypeObjectId objectId,
            ArgTypeBrowse browse,
            ArgTypeFilter filter,
            UInt32 startingIndex,
            UInt32 requestedCount,
            ArgTypeSortCriteria sortCriteria
        );
    }
}
