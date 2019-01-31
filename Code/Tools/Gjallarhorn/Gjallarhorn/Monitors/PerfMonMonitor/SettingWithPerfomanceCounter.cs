using System.Diagnostics;

namespace Gjallarhorn.Monitors.PerfMonMonitor
{
    public class SettingWithPerfomanceCounter
    {
        public PeformanceCounterSetting Setting { get; }
        public PerformanceCounter Counter { get; }

        public SettingWithPerfomanceCounter(PeformanceCounterSetting setting, PerformanceCounter counter)
        {
            Setting = setting;
            Counter = counter;
        }
    }
}
