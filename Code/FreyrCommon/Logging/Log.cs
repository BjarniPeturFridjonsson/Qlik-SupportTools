using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FreyrCommon.Logging
{

    public static class Log
    {
        private const string NAME = "Log";

        private static LogLevel _logLevel = LogLevel.None;
        private static string _logDir = "";
        private static string _logName = "";
        private static Thread _logThread;
        private static bool _running;
        private static readonly Queue<LogMessage> _logBuffer = new Queue<LogMessage>();
        private static readonly object _threadSyncObj = new object();

        public static event Action<string> NewLogItem;

        private static void StartLogThread()
        {
            try
            {
                lock (_threadSyncObj)
                {
                    _logThread?.Abort();
                    _running = true;
                    _logThread = new Thread(LogThread) { IsBackground = true, Name = "Log" };
                    _logThread.Start();
                }
            }
            catch (Exception ex)
            {
                AddToEventLog(NAME, ex);
                _running = false;
            }
        }

        private static void OnNewLogItem(string text)
        {
            try // we don't want a faulty event handler to crash our log-thread
            {
                NewLogItem?.Invoke(text);
            }
            catch (Exception ex)
            {
                AddToEventLog(NAME, ex);
            }
        }

        private static void LogThread()
        {
            try
            {
                while (_running || HasContent())
                {
                    var textLines = new StringBuilder();

                    LogMessage message;
                    while (Dequeue(out message))
                    {
                        if (message.LogLevel > _logLevel)
                        {
                            continue;
                        }

                        string text = message.DateTime.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fff' - '") + message.Text;
                        textLines.AppendLine(text);

                    }

                    if (textLines.Length > 0)
                    {
                        DateTime tryUntil = DateTime.UtcNow.AddSeconds(3);
                        while (DateTime.UtcNow < tryUntil)
                        {
                            try
                            {
                                string path = Path.Combine(_logDir, _logName);
                                File.AppendAllText(path, textLines.ToString(), Encoding.UTF8);
                                break;
                            }
                            catch
                            {
                                Thread.Sleep(2);
                            }
                        }
                    }

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                if (_running)
                {
                    Thread.Sleep(1000); //tight loop on io exeption(disk full).

                    StartLogThread();
                }

                AddToEventLog(NAME, ex);
            }
        }

        private static bool HasContent()
        {
            lock (_logBuffer)
            {
                return _logBuffer.Count > 0;
            }
        }

        private static bool Dequeue(out LogMessage logMessage)
        {
            lock (_logBuffer)
            {
                if (_logBuffer.Count > 0)
                {
                    logMessage = _logBuffer.Dequeue();
                    return true;
                }

                logMessage = null;
                return false;
            }
        }

        public static void Init(string logPath, LogLevel logLevel = LogLevel.Hardcore)
        {
            lock (_threadSyncObj)
            {
                if (logLevel == LogLevel.None)
                {
                    return;
                }

                if (_logThread != null)
                {
                    return; // don't re-initiate!
                }

                string dir = Path.GetDirectoryName(logPath);

                if (!Directory.Exists(dir))
                {
                    try
                    {
                        //this shoud never happen, but our logging framework is used in tools too that could potentially not have write rights to the folder.
                        Directory.CreateDirectory(dir + "");
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine($"We could not create logging directory. No logging will be done on disk. {e}");
                    }

                }

                _logDir = Path.GetDirectoryName(logPath);
                _logName = Path.GetFileName(logPath);

                _logLevel = logLevel;
                Add("Logging started");
                StartLogThread();
            }
        }

        public static void Shutdown()
        {
            Add("Stopping logging");
            //flushing the system.
            var now = DateTime.Now + TimeSpan.FromSeconds(10);
            while (HasContent())
            {
                Task.Delay(100).ConfigureAwait(false);
                if (DateTime.Now > now) break;
            }
            lock (_threadSyncObj)
            {
                _running = false;
                _logThread = null;
            }
        }

        public static void Add(string text, LogLevel level = LogLevel.Normal)
        {

            Trace.WriteLine($"Logging=>{text}");
            var message = new LogMessage(text, level, false);

            lock (_logBuffer)
            {
                _logBuffer.Enqueue(message);
            }
        }

        public static void Add(string name, Exception exception)
        {
            Add(name + ":" + exception.GetNestedMessages() + "\r\n" + exception.GetStackTrace());
        }

        public static void AddToEventLog(string name, string text)
        {
            AddToEventLog(name, text, EventLogEntryType.Information);
        }

        public static void AddToEventLog(string name, Exception exception)
        {
            AddToEventLog(name, exception.GetNestedMessages() + "\r\n" + exception.GetStackTrace(), EventLogEntryType.Error);
        }

        private static void AddToEventLog(string name, string text, EventLogEntryType type)
        {
            string prefix = string.IsNullOrEmpty(name) ? "" : name + ": ";

            try
            {
                EventLog.WriteEntry("Qlik Proactive Service", prefix + text, type);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
                //Add(NAME, new Exception(prefix + "Failed to write to Event log: " + text, exception), false);
            }
        }

        private class LogMessage
        {
            public LogMessage(string text, LogLevel logLevel, bool notifyEvent)
            {
                Text = text;
                LogLevel = logLevel;
                DateTime = DateTime.UtcNow;
            }

            public string Text { get; }

            public LogLevel LogLevel { get; }

            public DateTime DateTime { get; }
        }
    }
}