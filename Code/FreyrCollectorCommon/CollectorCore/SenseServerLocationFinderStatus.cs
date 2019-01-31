using System.ComponentModel;

namespace FreyrCollectorCommon.CollectorCore
{
    public enum SenseServerLocationFinderStatus
    {
        Undefined = 0,
        [Description("Server accessed successfully")]
        Success = 1,
        [Description("No valid Qlik Sense certificate found")]
        NoSertificateFound = 2,
        [Description("You are forbidden accessing this server")]
        Forbidden = 3,
        [Description("We can't find this Qlik Sense server")]
        NotAccessable = 4,
        [Description("We can't access this Qlik Sense server")]
        UnknownFailure = 5
    }
}
