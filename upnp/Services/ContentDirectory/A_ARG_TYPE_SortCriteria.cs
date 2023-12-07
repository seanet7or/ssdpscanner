namespace upnp.Services.ContentDirectory
{
    public class ArgTypeSortCriteria : List<string>
    {
        public static ArgTypeSortCriteria NoSort
        {
            get { return []; }
        }

        public override string ToString()
        {
            return string.Join(",", this);
        }
    }
}
