using System.Collections.Generic;

namespace Gjallarhorn.SenseLogReading.FileMiners
{
    public class ActiveFileMiners
    {
        private static List<IDataMiner> CurrentFileMiners => new List<IDataMiner>
        {
            new AuditActivityRepositoryMiner(),
            new AuditActivityProxyMiner()
        };

        public static List<IDataMiner> Get = CurrentFileMiners;
    }
}
