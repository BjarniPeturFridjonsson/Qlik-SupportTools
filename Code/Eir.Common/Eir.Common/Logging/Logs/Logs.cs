using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Eir.Common.Common;
using Eir.Common.IO;
using Eir.Common.Time;

namespace Eir.Common.Logging
{
    public class Logs : ILogs
    {
        private readonly ITrigger _logArchiverTrigger;
        private readonly LogArchiver _logArchiver;
        private string _logDir;
        private Func<string, LogFileNameComposer> _getLogFileNameComposer;
        private IFileSystem _fileSystem;
        private readonly Dictionary<Type, object> _dynamicLogs = new Dictionary<Type, object>();

        public Logs(
            string logDir,
            LogLevel logLevel,
            Func<bool> isTelemetryActive,
            ApplicationName applicationName,
            IFileSystem fileSystem = null,
            IDateTimeProvider dateTimeProvider = null)
        {
            if (fileSystem == null)
            {
                fileSystem = FileSystem.Singleton;
            }

            if (dateTimeProvider == null)
            {
                dateTimeProvider = DateTimeProvider.Singleton;
            }

            _getLogFileNameComposer = logName => new LogFileNameComposer(applicationName.Short, logName, ".txt");
            _logDir = logDir;
            _fileSystem = fileSystem;
            Main = new MainLog(_logDir, _getLogFileNameComposer("Log"), logLevel, fileSystem);

            Http = new HttpLog(_logDir, _getLogFileNameComposer("HttpLog"), fileSystem);

            Telemetry = new Telemetry(_logDir, _getLogFileNameComposer("TelemetryLog"), isTelemetryActive, fileSystem);

            WindowsEvent = new WindowsEventLog(applicationName.Display);

            Dump = new DumpLog(_logDir);

            _logArchiverTrigger = new PauseTrigger(() => TimeSpan.FromHours(1), TimeSpan.FromMinutes(15));

            _logArchiver = new LogArchiver(_logDir, 7, fileSystem, _getLogFileNameComposer("*"), dateTimeProvider, _logArchiverTrigger);
        }

        public void Dispose()
        {
            _logArchiverTrigger.Dispose();
            _logArchiver.Dispose();

            ((MainLog)Main).Dispose();
            ((HttpLog)Http).Dispose();
            ((Telemetry)Telemetry).Dispose();
            foreach (var item in _dynamicLogs.Values.OfType<IDisposable>())
            {
                item.Dispose();
            }
        }
        
        public void InitializeDynamicLog<T>(string logName, T headerLine) where T :LogItem
        {
            var log = new DynamicLog<T>(_logDir, _getLogFileNameComposer(logName), _fileSystem, headerLine);
            _dynamicLogs.Add(typeof(T),log);
        }

        public void AddToDynamicLog<T>(T logItem) where T : LogItem
        {
            object logObject;
            if (!_dynamicLogs.TryGetValue(typeof(T), out logObject))
            {
                Log.To.Main.Add($"Dynamic log access but not initalized {typeof(T)}");
                return;
            }
            var log = (IDynamicLog<T>)logObject;
            log.Add(logItem);
        }

        public IMainLog Main { get; }

        public IHttpLog Http { get; }

        public IWindowsEventLog WindowsEvent { get; }

        public IDumpLog Dump { get; }

        public ITelemetry Telemetry { get; }
    }
}