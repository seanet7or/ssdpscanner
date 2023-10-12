namespace upnp.Services.ContentDirectory
{
    public class ArgTypeObjectId
    {
        readonly string objectId;

        public override string ToString()
        {
            return objectId;
        }

        public ArgTypeObjectId(string objectId)
        {
            this.objectId = objectId;
        }

        public static ArgTypeObjectId RootFolder()
        {
            return new ArgTypeObjectId("0");
        }
    }
}
