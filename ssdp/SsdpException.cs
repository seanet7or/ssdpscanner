namespace ssdp
{
    public class SsdpException(string message, Exception? innerException)
        : Exception(message, innerException)
    {
        public SsdpException(string message)
            : this(message, null) { }
    }
}
