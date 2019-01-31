namespace Eir.Common.Common
{
    public class ApplicationName
    {
        //Major tools
        public static readonly ApplicationName ApplicationUpdater = new ApplicationName("ApplicationUpdater", "Qlik Tools Application Updater");
        public static readonly ApplicationName QlikCockpit = new ApplicationName("QlikCockpit", "Qlik Cockpit");
        public static readonly ApplicationName CollectiveAnalyzer = new ApplicationName("CollectiveAnalyzer", "Collective Analyzer");
        //public static readonly ApplicationName BifrostSyncService = new ApplicationName("BifrostSyncService", "Qlik Proactive Sync Service");
        //public static readonly ApplicationName BifrostBridgeService = new ApplicationName("BifrostBridgeService", "Qlik Proactive Bridge");
        //public static readonly ApplicationName BifrostService = new ApplicationName("BifrostService", "Qlik Proactive Data Receiver");
        public static readonly ApplicationName Gjallarhorn = new ApplicationName("Gjallarhorn", "Qlik Proactive Tools Service Monitor");
        //public static readonly ApplicationName Loke = new ApplicationName("LokeService", "Qlik Proactive Replay");
        //public static readonly ApplicationName MimirService = new ApplicationName("MimirService", "Qlik Proactive SelfService Portal");
        //public static readonly ApplicationName Saga = new ApplicationName("Saga", "Qlik Proactive Anomaly Detection");
        //public static readonly ApplicationName SagaDesktop = new ApplicationName("SagaDesktop", "Qlik Proactive Desktop");
        //public static readonly ApplicationName ValhallaService = new ApplicationName("ValhallaService", "Qlik Proactive Admin");

        //minor tools
        //public static readonly ApplicationName RemoveOldQvdFiles = new ApplicationName("RemoveOldQvdFiles", "Remove old qvd files");

        private ApplicationName(string shortName, string displayName)
        {
            Short = shortName;
            Display = displayName;
        }

        public string Short { get; }

        public string Display { get; }

        public override string ToString() => Short;
    }
}