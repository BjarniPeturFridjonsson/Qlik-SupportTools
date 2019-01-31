using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Eir.Common.Common;
using Eir.Common.Extensions;
using Eir.Common.Logging;
using Gjallarhorn.Common;
using Gjallarhorn.Notifiers;

namespace Gjallarhorn.Monitors.PerfMonMonitor
{
    internal class PerfMonWorker
    {
        //private const int PERFORMANCE_COUNTER_DETAIL_LOGGING_INTERVAL_IN_MINUTES = 5;
        //private readonly Action<string> _notify;
        //private bool _logPerformanceCounterDetails;
        //private DateTime _latestPerformanceCounterDetailLogTime;

        //private readonly AutoResetEvent _stopWaitHandle = new AutoResetEvent(false);
        //private readonly ConcurrentDictionary<string, PendingAlarm> _alarmThresholds = new ConcurrentDictionary<string, PendingAlarm>();
        private readonly ConcurrentDictionary<string, SettingWithPerfomanceCounter> _performanceCounters = new ConcurrentDictionary<string, SettingWithPerfomanceCounter>();

     

        //private static readonly char _valueSplitCharacter = ',';
        //private readonly Action<AdsRuleSetting> _resetNotification;
        //private bool _hasFiredState;
        //private bool _hasTriggerdState;

        private GroupedNotifyerWorker _groupedNotifyer;
        private readonly string _machineName;


        public PerfMonWorker(string machineName, Action<string, List<string>, string> notifier, Action<AdsRuleSetting> resetNotification)
        {
            //_notify = notifier;
            //_resetNotification = resetNotification;
            _groupedNotifyer = new GroupedNotifyerWorker("PerformanceMonitor.", notifier,null);
            _machineName = machineName;
        }

        public void PerformMonitoringForMachine()
        {
            HashSet<string> observedKeys = new HashSet<string>();
            //_hasTriggerdState = false;

            //IEnumerable<PeformanceCounterSetting> peformanceCounterSettings = GetPeformanceCounterSettings(_machineName);
            var keys = Settings.GetSetting($"PerformanceMonitor.{_machineName}.PerfCounters").Split(',').ToList();
            keys.ForEach(p =>
            {
                var setting = GetPeformanceCounterSettings(_machineName,p);
                var key = $"PerformanceMonitor.{_machineName}.{p}";
                _groupedNotifyer.AddIfMissing(key, $"Performance monitor detected that {setting.Name} on {_machineName} is looking bad for {setting.Duration} sec.", key);
                _groupedNotifyer.Analyze(key, CheckPerformanceCounter(setting));
                observedKeys.Add(GetCounterKey(setting));
            });
            _groupedNotifyer.AnalyzeRoundFinished();
            //foreach (var setting in peformanceCounterSettings)
            //{
            //    var key = $"PerformanceMonitor.{_machineName}";
            //    _groupedNotifyer.AddIfMissing(key, $"Performance monitor detected that {setting.Name} on {_machineName} is looking bad for {setting.Duration} sec.");
            //    _groupedNotifyer.Analyze(key, CheckPerformanceCounter(setting)); 
            //    observedKeys.Add(GetCounterKey(setting));
            //}

            //cleanup
            RemoveUnusedPerformanceCounters(observedKeys);
            //if (!_hasTriggerdState && _hasFiredState)
            //{
            //    _hasFiredState = false;
            //    _resetNotification(null);
            //}
        }
        //public void PerformMonitoring()
        //{
        //    //SetPerformanceCounterDetailLogging();
        //    HashSet<string> observedKeys = new HashSet<string>();
        //    _hasTriggerdState = false;
        //    var machineNames = GetMachineNames();
        //    foreach (var machineName in machineNames)
        //    {
        //        //Log.To.Main.Add($"Checking state of machine {machineName}.", LogLevel.Verbose);
        //        PerformMonitoringForMachine(machineName, observedKeys);
        //    }

        //    RemoveUnusedPerformanceCounters(observedKeys);
        //    if (!_hasTriggerdState && _hasFiredState)
        //    {
        //        _hasFiredState = false;
        //        _resetNotification(null);
        //    }
        //}

        //private void SetPerformanceCounterDetailLogging()
        //{
        //    var now = DateTimeProvider.Singleton.Time();
        //    _logPerformanceCounterDetails = now.Subtract(_latestPerformanceCounterDetailLogTime).TotalMinutes >= PERFORMANCE_COUNTER_DETAIL_LOGGING_INTERVAL_IN_MINUTES;
        //    if (_logPerformanceCounterDetails)
        //    {
        //        _latestPerformanceCounterDetailLogTime = now;
        //    }
        //}

        private void RemoveUnusedPerformanceCounters(HashSet<string> observedKeys)
        {
            PeformanceCounterSetting[] unobservedPerformanceCounterSettings = _performanceCounters
                .Where(pair => !observedKeys.Contains(pair.Key))
                .Select(pair => pair.Value.Setting)
                .ToArray();

            foreach (PeformanceCounterSetting setting in unobservedPerformanceCounterSettings)
            {
                RemoveSettingWithCounter(setting);
            }
        }

       

        private static string GetPerformanceCounterDisplayString(PeformanceCounterSetting setting)
        {
            return $"{setting.Category}\\{setting.Name}{(!string.IsNullOrWhiteSpace(setting.Instance) ? "(" + setting.Instance + ")" : string.Empty)}";
        }

        private int CheckPerformanceCounter(PeformanceCounterSetting setting)
        {
            try
            {
               // var ruleValidator = new RuleValidator();
                SettingWithPerfomanceCounter settingWithCounter = GetPerformanceCounter(setting);

                //if (_logPerformanceCounterDetails)
                //{
                //    LogPerformanceCounterDetails(settingWithCounter);
                //}

                var nextValue = (int)settingWithCounter.Counter.NextValue();
                return nextValue;
                //if (ruleValidator.ValidateValue(nextValue, settingWithCounter.Setting.ValidationExpression))
                //{
                //    SetAlarm(settingWithCounter.Setting, GetAlarmMessage(setting, settingWithCounter));
                //}
                //else
                //{
                //    ResetAlarm(settingWithCounter.Setting);
                //}
            }
            catch (Exception ex)
            {
                var message = $"Failure when trying to read performance counter '{GetPerformanceCounterDisplayString(setting)}' on machine {setting.MachineName}: {ex.GetNestedMessages()}";

                //SetAlarm(setting, message);
                Log.To.Main.Add(message,LogLevel.Error);
                return -1;
            }
        }

        //private string GetAlarmMessage(PeformanceCounterSetting setting, SettingWithPerfomanceCounter settingWithCounter)
        //{
        //    return string.Format("The test '{0}' has triggered for performance counter '{1}' on machine {2} has failed the test '{2}'.",
        //        settingWithCounter.Setting.ValidationExpression, GetPerformanceCounterDisplayString(setting), setting.MachineName);
        //}

        //private void LogPerformanceCounterDetails(SettingWithPerfomanceCounter settingWithCounter)
        //{
        //    Log.To.Main.Add($"Checking performance counter '{GetPerformanceCounterDisplayString(settingWithCounter.Setting)}' on machine {settingWithCounter.Setting.MachineName}. Validation expression: '{settingWithCounter.Setting.ValidationExpression}', Duration: {settingWithCounter.Setting.Duration}",LogLevel.Verbose);
        //}

        private SettingWithPerfomanceCounter GetPerformanceCounter(PeformanceCounterSetting setting)
        {
            var counterKey = GetCounterKey(setting);
            var storedSettingWithCounter = _performanceCounters.GetOrAdd(counterKey, key => CreateSettingWithCounter(setting));

            if (!storedSettingWithCounter.Setting.Equals(setting))
            {
                RemoveSettingWithCounter(setting);
                storedSettingWithCounter = GetPerformanceCounter(setting);
            }

            return storedSettingWithCounter;
        }

        private SettingWithPerfomanceCounter CreateSettingWithCounter(PeformanceCounterSetting setting)
        {
            Log.To.Main.Add($"Adding performance counter {GetPerformanceCounterDisplayString(setting)} on machine {setting.MachineName}. Validation expression: '{setting.ValidationExpression}', Duration: {setting.Duration}");

            return new SettingWithPerfomanceCounter(setting, new PerformanceCounter(setting.Category, setting.Name, setting.Instance, setting.MachineName));
        }
        
        private void RemoveSettingWithCounter(PeformanceCounterSetting setting)
        {
            var counterKey = GetCounterKey(setting);
            if (_performanceCounters.TryRemove(counterKey, out var oldSettingWithCounter))
            {
                oldSettingWithCounter.Counter.Dispose();
                Log.To.Main.Add($"Removed performance counter {GetPerformanceCounterDisplayString(oldSettingWithCounter.Setting)} on machine {oldSettingWithCounter.Setting.MachineName}. Validation expression: '{oldSettingWithCounter.Setting.ValidationExpression}', Duration: {oldSettingWithCounter.Setting.Duration}");
            }
        }

      

        private static string GetCounterKey(PeformanceCounterSetting setting)
        {
            return setting.MachineName + setting.Category + setting.Name + setting.Instance;
        }

        //private void ResetAlarm(PeformanceCounterSetting setting)
        //{
        //    var counterKey = GetCounterKey(setting);
        //    _alarmThresholds.TryRemove(counterKey, out PendingAlarm oldValue);
        //    Debug.WriteLine(oldValue);
        //}

        //private void SetAlarm(PeformanceCounterSetting setting, string text)
        //{
        //    var counterKey = GetCounterKey(setting);
        //    var now = DateTimeProvider.Singleton.Time();

        //    Func<string, PendingAlarm> addAlarmFunc = s => new PendingAlarm(now.Add(setting.Duration));
        //    _hasTriggerdState = true;
        //    PendingAlarm alarm = _alarmThresholds.GetOrAdd(counterKey, addAlarmFunc);
        //    if(alarm.StringBuilder.Length < 1)
        //        alarm.StringBuilder.AppendLine($"{text}");

        //    if (alarm.ThresholdTime <= now)
        //    {
        //        // sound the alarm
        //        string message = alarm.StringBuilder.ToString();
        //        Log.To.Main.Add("ALARM - " + message);
        //        _hasFiredState = true;
        //        _notify(message);

        //        // reset the alarm
        //        ResetAlarm(setting);

        //        // re-add the alarm; this performance counter measurement acts as new trigger
        //        _alarmThresholds.TryAdd(counterKey, addAlarmFunc(counterKey));
        //    }
        //}

        private PeformanceCounterSetting GetPeformanceCounterSettings(string machineName,string key)
        {
            //var keys = Settings.GetSetting($"PerformanceMonitor.{machineName}.PerfCounters").Split(',');

            //var values = keys.Select(key => new PeformanceCounterSetting(
            var item = new PeformanceCounterSetting(
                machineName:machineName,
                category:Settings.GetSetting($"PerformanceMonitor.{machineName}.{key}.Category"),
                name:Settings.GetSetting($"PerformanceMonitor.{machineName}.{key}.Name"),
                instance:Settings.GetSetting($"PerformanceMonitor.{machineName}.{key}.Instance"),
                validationExpression:Settings.GetSetting($"PerformanceMonitor.{machineName}.{key}.ValidationExpression"),
                duration: Settings.GetTimeSpan($"PerformanceMonitor.{machineName}.{key}.DurationInSeconds", TimeSpanSerializedUnit.Seconds, TimeSpan.FromSeconds(10)));

          
            if (
                string.IsNullOrWhiteSpace(item.Category) ||
                string.IsNullOrWhiteSpace(item.Name) ||
                string.IsNullOrWhiteSpace(item.ValidationExpression) ||
                string.IsNullOrWhiteSpace(item.Instance)
            )
            {
                Log.To.Main.Add("Failed validation of PerformanceMonitor agent settings", LogLevel.Error);
                return null;
            }
            return item;
        }

        public void Stop()
        {
            Log.To.Main.Add("Stopping monitoring agent");
            foreach (var item in _performanceCounters)
            {
                RemoveSettingWithCounter(item.Value.Setting);
            }
            Log.To.Main.Add("Stopped monitoring agent");
            //_stopWaitHandle.Set();
        }
    }
}