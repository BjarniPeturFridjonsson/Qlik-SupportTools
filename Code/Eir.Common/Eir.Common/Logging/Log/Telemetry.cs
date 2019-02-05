using System;
using Eir.Common.IO;
using Eir.Common.Time;

namespace Eir.Common.Logging
{
    public class Telemetry : ITelemetry, IDisposable
    {
        private readonly FileWriterLogItemHandler<TelemetryLogItem> _fileWriterLogItemHandler;
        private readonly QueuedLogItemHandler<TelemetryLogItem> _queuedLogItemHandler;
        private readonly ITrigger _trigger;
        private readonly Func<bool> _isActive; 

        public Telemetry(string logDir, LogFileNameComposer logFileNameComposer, Func<bool> isActive, IFileSystem fileSystem)
        {
            _isActive = isActive;
            _fileWriterLogItemHandler = new FileWriterLogItemHandler<TelemetryLogItem>(logDir, logFileNameComposer, fileSystem, TelemetryLogItem.Header);

            _trigger = new PauseTrigger(() => TimeSpan.FromSeconds(1));

            _queuedLogItemHandler = new QueuedLogItemHandler<TelemetryLogItem>(_fileWriterLogItemHandler, _trigger);
        }

        public void Dispose()
        {
            _trigger.Dispose();
            _queuedLogItemHandler.Dispose();
            _fileWriterLogItemHandler.Dispose();
        }

        public void Add(Func<string> getMessage, [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null)
        {
            if (!_isActive())
            {
                return;
            }

            var logItem = new TelemetryLogItem(
                DateTime.UtcNow,
                LogLevel.Info,
                getMessage(),
                callerFilePath);

            _queuedLogItemHandler.Add(logItem);
        }
    }
}