using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Eir.Common.Common;
using Eir.Common.Extensions;
using Eir.Common.Logging;
using Gjallarhorn.Common;
using Gjallarhorn.Monitors;
using Gjallarhorn.Notifiers;

namespace Gjallarhorn
{
    internal class Engine
    {
 
        //private MailerDaemon _mailerDaemon;
        private readonly AutoResetEvent _stopWaitHandle = new AutoResetEvent(false);

        private IEnumerable<INotifyerDaemon> _msgNotifier;

        private List<IGjallarhornMonitor> _regularMonitors;
        //private MailerDaemon _mailerDaemon;

        public void Start()
        {

            Log.To.Main.Add("Starting engine");
            //SecurityInitiator.InitProtocols();
            //_mailerDaemon = new MailerDaemon();
            //_mailerDaemon.Start();
            _regularMonitors = new List<IGjallarhornMonitor>();

            //Todo: function that will generate the Notification array based on the name of the monitor and inject that func into the Notifyer base.
                var notifyerListFactory = new NotifyerListFactory();


#if DEBUG
            //var test = new SenseLogFileParserMonitor(notifyerListFactory.NotifyerListCreator());
            //test.Execute();
            //test.Execute();
            //test.Execute();
            //test.Execute();
            //test.Execute();

            var test = new SenseStatisticsMonitor(notifyerListFactory.NotifyerListCreator());
            test.Execute();

#endif

            _regularMonitors.Add(new FilesMonitor(notifyerListFactory.NotifyerListCreator()));
            _regularMonitors.Add(new PerformanceMonitor(notifyerListFactory.NotifyerListCreator()));
            _regularMonitors.Add(new DiskDrivesMonitor(notifyerListFactory.NotifyerListCreator()));
            _regularMonitors.Add(new MemoryMonitor(notifyerListFactory.NotifyerListCreator()));
            _regularMonitors.Add(new SenseNodeStatusMonitor(notifyerListFactory.NotifyerListCreator()));
            _regularMonitors.Add(new SenseTaskMonitor(notifyerListFactory.NotifyerListCreator()));
            _regularMonitors.Add(new QmsMonitor(notifyerListFactory.NotifyerListCreator()));
            _regularMonitors.Add(new SenseStatisticsMonitor(notifyerListFactory.NotifyerListCreator()));
            _regularMonitors.Add(new QlikViewStatisticsMonitor(notifyerListFactory.NotifyerListCreator()));
            _regularMonitors.Add(new SenseLogFileParserMonitor(notifyerListFactory.NotifyerListCreator()));
            _regularMonitors.Add(new QlikViewLogFileParserMonitor(notifyerListFactory.NotifyerListCreator()));

            //digest stuff

            _msgNotifier = notifyerListFactory.NotifyerListCreator().Invoke("General");

#if DEBUG
            //monitorHttpLogs.GetDigestMessages();
            //senseTaskMonitor.Execute();
            //_mailerDaemon.EnqueueMessage("this is a test");
            //monitorCustomerDb.Execute();
            //monitorMainLogs.Execute();
#endif

            Thread workerThread = new Thread(Worker) { IsBackground = true };
            workerThread.Start();
        }

        private TimeSpan GetNextInterval()
        {
            return Settings.GetTimeSpan("General.WorkIntervalInSeconds", TimeSpanSerializedUnit.Seconds, TimeSpan.FromSeconds(30));
        }

        private TimeSpan GetSpecificInterval(string settingsObjectName)
        {   
            return Settings.GetTimeSpan($"{settingsObjectName}.IntervalInSeconds", TimeSpanSerializedUnit.Seconds, TimeSpan.FromHours(4));
        }

        private void Worker()
        {
            TimeSpan delay = TimeSpan.Zero;
            DateTime nextMessageSender = DateTimeProvider.Singleton.Today.AddHours(7.5 + 24);
            foreach (var notifyerDaemon in _msgNotifier)
            {
                notifyerDaemon.EnqueueMessage("", $"Qlik Monitoring tool startup detected on {Environment.MachineName}.");
            }
            while (true)
            {

                // If the wait handle is signalled, we should stop working.
                // If not, perform one work iteration, then wait again.
                bool stopWorking = _stopWaitHandle.WaitOne(delay);
                if (stopWorking)
                {
                    break;
                }
                try
                {
                    _regularMonitors.ForEach(monitor =>
                    {
                        if (string.IsNullOrWhiteSpace(monitor.MonitorName))
                        {
                            Log.To.Main.Add($"Monitor does not have a name, That is bad. this won't work for {monitor.ToString()}", LogLevel.Error);
                        }
                        else
                        {
                            if (monitor.NextExec < DateTimeProvider.Singleton.Time() && IsMonitorEnabled(monitor.MonitorName))
                            {
                                monitor.NextExec = DateTimeProvider.Singleton.Time() + GetSpecificInterval(monitor.MonitorName);
                                try
                                {
                                    monitor.Execute();
                                }
                                catch (Exception e)
                                {
                                    Log.To.Main.Add($"The {monitor.ToString()} threw an unhandled exception  {e}", LogLevel.Error);
                                }

                            }
                        }

                    });

                    if (nextMessageSender < DateTimeProvider.Singleton.Time())
                    {
                        nextMessageSender = DateTimeProvider.Singleton.Today.AddHours(7.5 + 24);
                        _regularMonitors.ForEach(p =>
                        {
                            var msg = p.GetDigestMessages();
                            if (!string.IsNullOrEmpty(msg))
                            {
                                foreach (var notifyerDaemon in _msgNotifier)
                                {
                                    notifyerDaemon.EnqueueMessage(p.MonitorName, msg);
                                }
                            }
                                
                        });

                    }

                    // ...and get the next wait delay time from the settings
                    var nextDelay = GetNextInterval();

                    if (nextDelay != delay)
                    {
                        Log.To.Main.Add($"Changing delay times from {delay.TotalSeconds} to {nextDelay.TotalSeconds} seconds");
                        delay = nextDelay;
                    }
                }
                catch (Exception ex)
                {
                    Log.To.Main.Add($"Error when performing monitoring: {ex.GetNestedMessages()}");
                    Log.To.Main.Add(ex.StackTrace);
                }
            }

            Log.To.Main.Add("Leaving Engine worker thread");
            Stop();
        }

        private bool IsMonitorEnabled(string settingsObjectName)
        {
            var setting = Settings.GetSetting($"{settingsObjectName}.Enabled");
            return string.Compare(setting, "true", StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public string GetUserAgent(string appName)
        {
            return $"ProacticeMonitorService/{Assembly.GetExecutingAssembly().GetName().Version}; ({appName})";
        }

        public void Stop()
        {
            Log.To.Main.Add("Stopping engine");
            _regularMonitors?.ForEach(p => p.Stop());



            //if (_mailerDaemon != null)
            //{
            //    _mailerDaemon.Stop();
            //    _mailerDaemon = null;
            //}
        }
    }
}