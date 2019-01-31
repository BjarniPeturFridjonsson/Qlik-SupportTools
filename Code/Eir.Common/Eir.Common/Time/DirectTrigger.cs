using System.Threading.Tasks;

namespace Eir.Common.Time
{
    /// <summary>
    /// This trigger invokes the delgate immediately on demand.
    /// </summary>
    public class DirectTrigger : TriggerBase
    {
        public Task Trig()
        {
            return TrigInternal();
        }
    }
}