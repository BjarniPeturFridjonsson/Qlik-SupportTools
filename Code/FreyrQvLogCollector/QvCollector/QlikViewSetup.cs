using System.Collections.Generic;
using System.Linq;
using Eir.Common.IO;

namespace FreyrQvLogCollector.QvCollector
{
    public class QlikViewSetup
    {
        public QlikViewSetup(
            IEnumerable<QlikViewServiceInfo> serviceInfos,
            DirectorySetting userDocDirectory,
            DirectorySetting qvsLogDirectory,
            FileSetting qvsEventLogFile,
            FileSetting qvsSessionLogFile,
            DirectorySetting qdsDirectory)
        {
            ServiceInfos = serviceInfos.ToArray();
            UserDocDirectory = userDocDirectory;
            QvsLogDirectory = qvsLogDirectory;
            QvsEventLogFile = qvsEventLogFile;
            QvsSessionLogFile = qvsSessionLogFile;
            QdsDirectory = qdsDirectory;
        }

        public QlikViewServiceInfo[] ServiceInfos { get; }

        public DirectorySetting UserDocDirectory { get; }

        public DirectorySetting QvsLogDirectory { get; }

        public FileSetting QvsEventLogFile { get; }

        public FileSetting QvsSessionLogFile { get; }

        public DirectorySetting QdsDirectory { get; }
    }
}
