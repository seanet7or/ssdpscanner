namespace upnp.Services.ContentDirectory
{
    public class ArgTypeFilter : List<string>
    {
        public static ArgTypeFilter MatchAll()
        {
            return ["*"];
        }

        public override string ToString()
        {
            return string.Join(",", this);
        }
    }
}
