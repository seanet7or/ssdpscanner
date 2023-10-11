namespace utils.Logging
{
    public interface IFileAccess
    {
        Stream CreateForWriting(string fileName);
    }
}
