namespace upnp.Soap
{
    interface ISoapActionResponseArgument
    {
        string? Name { get; }

        string? Value { get; }

        UInt32 ParseUi4();
    }
}
