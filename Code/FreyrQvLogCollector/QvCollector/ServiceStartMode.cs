namespace FreyrQvLogCollector.QvCollector
{
    public enum ServiceStartMode
    {
        NotInstalled,
        Unknown,

        // The following comes from https://msdn.microsoft.com/en-us/library/aa394418(v=vs.85).aspx
        Boot,
        System,
        Auto,
        Manual,
        Disabled
    }
}