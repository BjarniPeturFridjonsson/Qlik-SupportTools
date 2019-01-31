using System;

namespace Gjallarhorn.Monitors
{
    public interface IGjallarhornMonitor
    {
        string MonitorName { get; }
        DateTime NextExec { get; set; }
        string GetDigestMessages();
        void Execute();
        void Stop();
    }
}
