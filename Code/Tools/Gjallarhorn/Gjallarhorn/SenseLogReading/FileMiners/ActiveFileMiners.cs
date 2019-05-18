using System.Collections.Generic;

namespace Gjallarhorn.SenseLogReading.FileMiners
{
    public class ActiveFileMiners
    {
        public List<IDataMiner> GetQlikSenseFileMiners()
        {
            return new List<IDataMiner>
            {
                //new AuditActivityRepositoryMiner(),
                new AuditActivityProxyMiner(),
            };
        }
    }
}
