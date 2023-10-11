namespace upnp
{
    public interface IHttpClient
    {
        Task<Stream> GetAsync(string acceptLanguage, string url);

        Task<Stream> PostDataAsync(Dictionary<string, string> headers, byte[] data, string url);
    }
}
