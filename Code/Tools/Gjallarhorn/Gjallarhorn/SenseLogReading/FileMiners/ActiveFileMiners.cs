using System.Collections.Generic;

namespace Gjallarhorn.SenseLogReading.FileMiners
{
    public class ActiveFileMiners
    {
        public static List<IDataMiner> Get => new List<IDataMiner>
        {
            new AuditActivityRepositoryMiner(),
            new AuditActivityProxyMiner()
        };
    }
}
