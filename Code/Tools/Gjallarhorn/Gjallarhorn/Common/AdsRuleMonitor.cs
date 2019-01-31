using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Eir.Common.Common;
using Eir.Common.Logging;

namespace Gjallarhorn.Common
{
    //todo:If the settings change this is not reflected in the validationExpressions.
    public class AdsRuleMonitor
    {
        public  AdsRuleSetting Settings { get; }

        private readonly ConcurrentDictionary<string, PendingAlarm> _alarmThresholds = new ConcurrentDictionary<string, PendingAlarm>();
        private readonly Action<AdsRuleSetting> _adsAlarmTrigger;
        private readonly Action<AdsRuleSetting> _adsResetTriggerAlarm;

        public AdsRuleMonitor(AdsRuleSetting settings, Action<AdsRuleSetting> adsAlarmTrigger, Action<AdsRuleSetting> adsResetTriggerAlarm)
        {
            Settings = settings;
            _adsAlarmTrigger = adsAlarmTrigger;
            _adsResetTriggerAlarm = adsResetTriggerAlarm;
        }

        public void Analyze(double nextValue)
        {
            var ruleValidator = new RuleValidator();
            Settings.ReadSettings();
            if (ruleValidator.ValidateValue(nextValue, Settings.ValidationExpression))
            {
                SetAlarm(Settings, nextValue);
            }
            else
            {
                ResetAlarm(Settings);
                if (Settings.IsInFiredCondition)
                {
                    Settings.IsInFiredCondition = false;
                    _adsResetTriggerAlarm.Invoke(Settings);
                }
            }
        }

        public void ForceResetTimer(DateTime utcTime)
        {
            _alarmThresholds[GetCounterKey(Settings)] = new PendingAlarm(utcTime.Add(Settings.Duration));
        }

        private string GetAlarmMessage(AdsRuleSetting setting)
        {
            return $"The Rule '{setting.AdsRuleName}' has triggered on machine {setting.MachineName}.";
        }

        private string GetCounterKey(AdsRuleSetting setting)
        {
            return setting.MachineName + '_' + setting.AdsRuleName;
        }

        private void ResetAlarm(AdsRuleSetting setting)
        {
            var counterKey = GetCounterKey(setting);
            _alarmThresholds.TryRemove(counterKey, out var oldValue);
            Debug.WriteLine(oldValue);
        }

        private void SetAlarm(AdsRuleSetting setting, double firedValue)
        {
            var counterKey = GetCounterKey(setting);
            var now = DateTimeProvider.Singleton.Time();

            Func<string, PendingAlarm> addAlarmFunc = s => new PendingAlarm(now.Add(setting.Duration));

            PendingAlarm alarm = _alarmThresholds.GetOrAdd(counterKey, addAlarmFunc);

            if (alarm.ThresholdTime <= now)
            {
                // sound the alarm
                string message = GetAlarmMessage(setting);
                setting.IsInFiredCondition = true;
                Log.To.Main.Add("ALARM - " + message);
                setting.AdsLastFired = new AdsRuleSetting.AdsRuleFiredDto { Fired = now, Value = firedValue };
                _adsAlarmTrigger.Invoke(setting);

                // reset the alarm
                ResetAlarm(setting);

                // re-add the alarm; this counter measurement acts as new trigger
                _alarmThresholds.TryAdd(counterKey, addAlarmFunc(counterKey));
            }
        }

        private void ResetValidation(AdsRuleSetting setting, string key, PendingAlarm alarm)
        {
            // reset the alarm
            ResetAlarm(setting);

            // re-add the alarm; this counter measurement acts as new trigger
            _alarmThresholds.TryAdd(key, alarm);
        }
    }
}
