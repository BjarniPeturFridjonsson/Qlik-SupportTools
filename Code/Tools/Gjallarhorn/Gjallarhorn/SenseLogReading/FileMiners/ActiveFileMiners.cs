using System.Collections.Generic;

namespace Gjallarhorn.SenseLogReading.FileMiners
{
    public class ActiveFileMiners
    {
        public List<IDataMiner> GetActiveFileMiners()
        {
            return new List<IDataMiner>
            {
                //new AuditActivityRepositoryMiner(),
                new AuditActivityProxyMiner(),
            };
        }
    }
}
