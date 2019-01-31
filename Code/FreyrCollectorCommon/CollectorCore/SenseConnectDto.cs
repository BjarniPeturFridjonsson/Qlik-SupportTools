using System;
using SenseApiLibrary;

namespace FreyrCollectorCommon.CollectorCore
{
    public class SenseConnectDto
    {
        public string SenseHostName { get; set; }
        public bool AbortAndExit { get; set; }
        public bool RunWithDeadInstallation { get; set; }
        public string PathToLocalSenseLogFolder { get; set; }
        public SenseApiSupport SenseApiSupport { get; set; }
        public SenseServerLocationFinderStatus SenseServerLocationFinderStatus { get; set; }
        public Func<SenseConnectDto, SenseConnectDto> ConnectToSenseApiManuallyDlg;
    }
}
