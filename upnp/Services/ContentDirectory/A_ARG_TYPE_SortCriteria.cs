namespace upnp.Services.ContentDirectory
{
    public class ArgTypeSortCriteria : List<string>
    {
        public static ArgTypeSortCriteria NoSort => [];

        public override string ToString()
        {
            return string.Join(",", this);
        }
    }
}
