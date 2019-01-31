using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gjallarhorn.Common
{
    /// <summary>
    /// IncrementOptionEnum used in Sense for tasks scheduling.
    /// </summary>
    public enum IncrementOptionEnum
    {
        Undefined = -1,
        Once = 0,
        Hourly = 1,
        Daily = 2,
        Weekly = 3,
        Monthly = 4
    }
}
