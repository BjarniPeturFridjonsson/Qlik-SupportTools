using System.ComponentModel;

namespace FreyrQvLogCollector.QvCollector
{
    public enum QlikViewServerLocationFinderStatus
    {
        Undefined = 0,
        [Description("Server accessed successfully")]
        Success = 1,
        [Description("You are forbidden accessing this server")]
        Forbidden = 3,
        [Description("We can't find this QlikView server")]
        NotAccessable = 4,
        [Description("We can't access this QlikView server")]
        UnknownFailure = 5
    }
}
