using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Eir.Common.Common;
using Eir.Common.Logging;
using Gjallarhorn.Common;

namespace Gjallarhorn.Notifiers
{
    public class GroupedNotifyerWorker
    {
        private readonly Dictionary<string, AdsRuleMonitor> _ruleMonitors;
        private readonly string _monitorName;
        private bool _firstTimeInRound = true;
        private string _previousRoundTriggeredHash;
        private Action<string, List<string>, string> _notify;
        private List<string> _listOfWarnings = new List<string>();
        private int _keyCounter;
        private Func<AdsRuleSetting, string> _messageModifier;

        public GroupedNotifyerWorker(string monitorName, Action<string, List<string>, string> notify, Func<AdsRuleSetting, string> messageModifier)
        {
            _ruleMonitors = new Dictionary<string, AdsRuleMonitor>();

            _monitorName = monitorName;
            _notify = notify;
            _messageModifier = messageModifier;
        }

        public void AddIfMissing(string key, string notifyerMsg, string overrideInternalSettingsKey ="")
        {
            if (!_ruleMonitors.ContainsKey(key))
            {
                _keyCounter++;
                var settingsKey = string.IsNullOrEmpty(overrideInternalSettingsKey) ? _monitorName : overrideInternalSettingsKey;
                _ruleMonitors.Add(key, new AdsRuleMonitor(new AdsRuleSetting(settingsKey, notifyerMsg), RuleTriggered, ResetNotificationList));
                _ruleMonitors[key].Settings.Key = _keyCounter.ToString();
            }
        }

        public void Analyze(string key, double nextValue)
        {
            if (_firstTimeInRound)
            {
                _firstTimeInRound = false;
                _previousRoundTriggeredHash = GetGroupTriggeredHash();
                _listOfWarnings = new List<string>();
                _ruleMonitors[key].Settings.ReadSettings();
            }
            _ruleMonitors[key].Analyze(nextValue);
        }

        public void AnalyzeRoundFinished()
        {
            if (_listOfWarnings.Count > 0)
            {
                if (_listOfWarnings.Count > 5)
                 {
                    var concatList = _listOfWarnings.Take(5).ToList();
                    concatList.Add($"Plus {_listOfWarnings.Count - concatList.Count} other warnings found.");
                    _notify($"{_monitorName} found {_listOfWarnings.Count} warnings.", concatList, "");
                }
                else
                {
                     _notify($"{_monitorName} has fired a warning", _listOfWarnings, "");
                }
                var currentTotalTriggered = GetGroupTriggeredHash();
                if (currentTotalTriggered.Equals(_previousRoundTriggeredHash))
                {//Warnings must trigger at the same time or the notifyier will spam.
                    var time = DateTimeProvider.Singleton.Time();
                    foreach (var item in _ruleMonitors.Values)
                    {
                        if (item.Settings.IsInFiredCondition)
                            item.ForceResetTimer(time);
                    }
                }
            }
            else
            {
                bool allOk = true;
                foreach (AdsRuleMonitor item in _ruleMonitors.Values)
                {
                    if (item.Settings.IsInFiredCondition)
                        allOk = false;
                }
                if (allOk)
                {
                    _notify("", new List<string>(), "");
                }
            }
            _firstTimeInRound = true;
        }

        private string GetGroupTriggeredHash()
        {
            var a = _ruleMonitors.Values.Where(p => p.Settings.IsInFiredCondition).Select(p => p.Settings.Key).ToList();
            if (!a.Any())
                return "";

            var hash = a.Aggregate((item, sum) => sum + "," + item);
            return hash;
        }

        public AdsRuleSetting GetSettings(string key)
        {
            if (_ruleMonitors.ContainsKey(key))
                return _ruleMonitors[key].Settings;
            return null;
        }

        private void RuleTriggered(AdsRuleSetting setting)
        {
            if (!string.IsNullOrEmpty(setting.NegativeFilter))
            {
                try
                {
                    var regex = new Regex(setting.NegativeFilter);
                    if (regex.IsMatch(setting.AdsRuleName))
                    {
                        Log.To.Main.Add($"Ignoring rule due to filtering '{setting.AdsRuleName}' on machine {setting.MachineName}",LogLevel.Verbose);
                        return;
                    }
                }
                catch (Exception e)
                {
                    Log.To.Main.AddException($"Failed regex in Grouped Notifier worker with {setting.Key} and regex {setting.NegativeFilter}",e);
                }    
            }
            //Notify("Sense node status Monitor has fired a warning", $"The Rule '{setting.AdsRuleName}' has triggered on machine {setting.MachineName}.");
            var msg = _messageModifier?.Invoke(setting) ?? $"The Rule '{setting.AdsRuleName}' has triggered on machine {setting.MachineName}.";
            _listOfWarnings.Add(msg);
        }

        private void ResetNotificationList(AdsRuleSetting setting)
        {

        }
    }
}
