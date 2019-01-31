using System;

namespace FreyrQvLogCollector.QvCollector
{
    public class QlikViewConnectDto
    {

        public string QmsAddress { get; set; }
        public bool ConnectionSuccess { get; set; }
        public bool AbortAndExit { get; set; }
        public bool QvManagementApiGroupDetected { get; set; }
        public bool RunWithDeadInstallation { get; set; }
        public string PathToLocalLogFolder { get; set; }
        public QlikViewServerLocationFinderStatus QlikViewServerLocationFinderStatus { get; set; }
        public Func<QlikViewConnectDto, QlikViewConnectDto> ConnectToQmsApiManuallyDlg;
    }
}
