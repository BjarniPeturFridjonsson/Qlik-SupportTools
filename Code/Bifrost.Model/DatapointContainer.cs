using System.Collections.Generic;
using System.Diagnostics;

namespace Bifrost.Model
{
    [DebuggerDisplay("DatapointContainer: Customer={Dps[0].CustomerId}, ContainerId={Dps[0].ContainerId}, {Dps.Count} datapoints ")]
    public class DatapointContainer
    {
        public List<Datapoint> Dps { get; set; }

        public DatapointContainer()
        {
            Dps = new List<Datapoint>();
        }
    }
}