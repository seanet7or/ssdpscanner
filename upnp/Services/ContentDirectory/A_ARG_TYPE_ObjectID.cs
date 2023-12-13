namespace upnp.Services.ContentDirectory
{
    public class ArgTypeObjectId(string objectId)
    {
        readonly string objectId = objectId;

        public override string ToString()
        {
            return objectId;
        }

        public static ArgTypeObjectId RootFolder()
        {
            return new ArgTypeObjectId("0");
        }
    }
}
