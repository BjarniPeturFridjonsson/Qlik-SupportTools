using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Eir.Common.Common;
using Eir.Common.Logging;
using Gjallarhorn.Common;
using Gjallarhorn.Monitors.QmsApi;
using Gjallarhorn.Notifiers;
using SenseApiLibrary;

namespace Gjallarhorn.Monitors
{
    public class SenseTaskMonitor : BaseMonitor, IGjallarhornMonitor
    {
        private SenseApiSupport _senseApiSupport;
        private readonly GroupedNotifyerWorker _groupedNotifyer;

        public SenseTaskMonitor(Func<string, IEnumerable<INotifyerDaemon>> notifyerDaemons) : base(notifyerDaemons, "SenseTaskMonitor")
        {
            //_ruleMonitors = new Dictionary<string, AdsRuleMonitor>();
            _groupedNotifyer = new GroupedNotifyerWorker(MonitorName, Notify, ModAlertString);
        }

        private string ModAlertString(AdsRuleSetting setting)
        {
            return (int)setting.AdsLastFired.Value == 2 ? 
                $"Task taken too long: '{setting.AdsRuleName}' on '{setting.MachineName}'" : 
                $"Task has failed reload '{setting.AdsRuleName}' on '{setting.MachineName}'";
        }

        public void Execute()
        {
            try
            {
                var host = Settings.GetSetting($"{MonitorName}.HostName", "localhost");
                if ((_senseApiSupport == null) || (_senseApiSupport.Host != host))
                {
                    _senseApiSupport = SenseApiSupport.Create(host);
                }

                SenseEnums senseEnums = new SenseEnums(_senseApiSupport);
                var taskHelper = new TaskHelper();
                var tasks = taskHelper.GetAllTasksFullDto(_senseApiSupport,senseEnums);
                var filter = new Regex(Settings.GetSetting($"{MonitorName}.Filter").Trim(), RegexOptions.IgnoreCase);
                var negativeFilter = new Regex(Settings.GetSetting($"{MonitorName}.NegativeFilter").Trim(), RegexOptions.IgnoreCase);

                foreach (var dto in tasks)
                {
                    if (filter.ToString() == string.Empty ||  filter.IsMatch(dto.Name) || filter.IsMatch(dto.Taskid.ToString()))
                    {
                        if (negativeFilter.ToString() != string.Empty && (negativeFilter.IsMatch(dto.Name) || negativeFilter.IsMatch(dto.Taskid.ToString())))
                            continue;
                    }
                    else
                    {
                        continue;
                    }
                    var key = dto.Taskid.ToString();
                    if (dto.Enabled.GetValueOrDefault(false))
                    {
                        //what bucket
                        var bucket = GetScheduledTimeBucket(dto);
                        TimeSpan? ruleDuration = null;
                        switch (bucket)
                        {
                            case IncrementOptionEnum.Hourly:
                                ruleDuration = TimeSpan.FromSeconds(Settings.GetInt32($"{MonitorName}.HourlyDurationInSeconds", 0));
                                break;
                            case IncrementOptionEnum.Daily:
                                ruleDuration = TimeSpan.FromSeconds(Settings.GetInt32($"{MonitorName}.DailyDurationInSeconds", 0));
                                break;
                            case IncrementOptionEnum.Weekly:
                                ruleDuration = TimeSpan.FromSeconds(Settings.GetInt32($"{MonitorName}.WeeklyDurationInSeconds", 0));
                                break;
                            case IncrementOptionEnum.Monthly:
                                ruleDuration = TimeSpan.FromSeconds(Settings.GetInt32($"{MonitorName}.MonthlyDurationInSeconds", 0));
                                break;
                        }

                        if (ruleDuration.GetValueOrDefault(TimeSpan.FromSeconds(0)) < TimeSpan.FromSeconds(1))
                            continue; // ignore buckets with no defined minimum execution duration
                        _groupedNotifyer.AddIfMissing(key, dto.Name);
                        
                        //check if the schedule has changed bucket.
                        if (_groupedNotifyer.GetSettings(key).Duration != ruleDuration)
                        {
                            _groupedNotifyer.GetSettings(key).Duration = ruleDuration.GetValueOrDefault(TimeSpan.FromSeconds(0));
                            continue; // don't analyze this time around. Material change rule duration.
                        }
                        _groupedNotifyer.Analyze(key, dto.Operational?.LastExecutionResult?.Status ?? -1);
                    }
                }
                _groupedNotifyer.AnalyzeRoundFinished();
            }
            catch (Exception ex)
            {
                _senseApiSupport = null;
                Log.To.Main.AddException($"Failed executing {MonitorName}", ex);
            }
        }

        private IncrementOptionEnum GetScheduledTimeBucket(TaskDto dto)
        {
            var lastExecStart = dto.Operational?.LastExecutionResult?.Starttime;
            var nextExec = dto.Operational?.NextExecution;
            if (lastExecStart == null || lastExecStart < DateTimeProvider.Singleton.Time().AddYears(-2) || nextExec < (DateTimeProvider.Singleton.Time().AddYears(-2)))
            {
                return IncrementOptionEnum.Undefined;
            }
            var span = nextExec - lastExecStart;
            if (span < TimeSpan.Zero) { return IncrementOptionEnum.Undefined;}
                

            if (nextExec - lastExecStart < TimeSpan.FromDays(1))
                return IncrementOptionEnum.Hourly;
            if (nextExec - lastExecStart < TimeSpan.FromDays(7))
                return IncrementOptionEnum.Daily;
            if (nextExec - lastExecStart < TimeSpan.FromDays(32))
                return IncrementOptionEnum.Weekly;
            
            return IncrementOptionEnum.Monthly;
        }
       
    }
}
