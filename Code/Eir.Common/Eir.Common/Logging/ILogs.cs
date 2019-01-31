using System;
using System.Collections.Generic;

namespace Eir.Common.Logging
{
    public interface ILogs : IDisposable
    {
        IMainLog Main { get; }

        IHttpLog Http { get; }

        IWindowsEventLog WindowsEvent { get; }

        IDumpLog Dump { get; }

        ITelemetry Telemetry { get; }

        void InitializeDynamicLog<T>(string logName, T headerLine) where T : LogItem;

        void AddToDynamicLog<T>(T logItem) where T : LogItem;
    }
}