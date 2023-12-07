namespace upnp.Soap
{
    class SoapActionResponseArgument : ISoapActionResponseArgument
    {
        public string? Name { get; internal set; }

        public string? Value { get; internal set; }

        public UInt32 ParseUi4()
        {
            if (UInt32.TryParse(Value, out uint result))
            {
                return result;
            }
            return 0;
        }
    }
}
