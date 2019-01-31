using System.Threading;

namespace Eir.Common.Extensions
{
    public static class TimerExtensions
    {
        public static void DoDispose(this Timer timer)
        {
            try
            {
                timer?.Dispose();
            }
            catch
            {
            }
        }
    }
}
