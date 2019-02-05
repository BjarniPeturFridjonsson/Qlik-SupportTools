using System;
using System.Diagnostics;
using System.Threading;
using Eir.Common.IO;
using Eir.Common.Time;

namespace Eir.Common.Logging
{
    public class MainLog : IMainLog, IDisposable
    {
        private readonly FileWriterLogItemHandler<MainLogItem> _fileWriterLogItemHandler;
        private readonly QueuedLogItemHandler<MainLogItem> _queuedLogItemHandler;
        private readonly ITrigger _trigger;
        private readonly LogLevel _logLevel;
         
        public MainLog(string logDir, LogFileNameComposer logFileNameComposer, LogLevel logLevel, IFileSystem fileSystem)
        {
            _logLevel = logLevel;

            _fileWriterLogItemHandler = new FileWriterLogItemHandler<MainLogItem>(logDir, logFileNameComposer, fileSystem, MainLogItem.Header);

            _trigger = new PauseTrigger(() => TimeSpan.FromSeconds(1));

            _queuedLogItemHandler = new QueuedLogItemHandler<MainLogItem>(_fileWriterLogItemHandler, _trigger);
        }

        public void Dispose()
        {
            _trigger.Dispose();
            _queuedLogItemHandler.Dispose();
            _fileWriterLogItemHandler.Dispose();
        }

        public event EventHandler<MainLogItemEventArgs> NewLogItem;

        public void Add(string message, LogLevel logLevel, string memberName, string filePath, int lineNumber)
        {
            if (logLevel > _logLevel)
            {
                return;
            }

            var logItem = new MainLogItem(
                DateTime.UtcNow,
                logLevel,
                Thread.CurrentThread.Name ?? $"<thread>_{Thread.CurrentThread.ManagedThreadId}",
                filePath,
                lineNumber.ToString(),
                memberName,
                message,
                null);

            Add(logItem);
        }

        public void AddException(string message, Exception exception, LogLevel logLevel, string memberName, string filePath, int lineNumber)
        {
            if (logLevel > _logLevel)
            {
                return;
            }

            var logItem = new MainLogItem(
                DateTime.UtcNow,
                logLevel,
                Thread.CurrentThread.Name ?? $"<thread>_{Thread.CurrentThread.ManagedThreadId}",
                filePath,
                lineNumber.ToString(),
                memberName,
                message,
                exception);

            Add(logItem);
        }

        private void Add(MainLogItem logItem)
        {
            _queuedLogItemHandler.Add(logItem);

            try
            {
                NewLogItem?.Invoke(this, new MainLogItemEventArgs(logItem));
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"EXCEPTION IN {nameof(MainLog)}: {exception}");
            }
        }
    }
}