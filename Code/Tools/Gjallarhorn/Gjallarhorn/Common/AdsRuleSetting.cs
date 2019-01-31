using System;
using Eir.Common.Common;

namespace Gjallarhorn.Common
{
    public class AdsRuleSetting
    {
        public class AdsRuleFiredDto
        {
            public double Value { get; set; }
            public DateTime Fired { get; set; }
            public DateTime Triggered { get; set; }
        }
        public bool IsInFiredCondition { get; set; }
        public string ValidationExpression { get; private set; }
        public string NegativeFilter { get; private set; }
        public TimeSpan Duration { get; set; }
        public string Key { get; set; }
        public string MachineName { get; }
        public string AdsRuleName { get; }
        public AdsRuleFiredDto AdsLastFired { get; set; }

        private readonly string _settingKey;

        public AdsRuleSetting(string settingKey, string ruleName, TimeSpan? ruleDuration = null)
        {
            ValidationExpression = Settings.GetSetting(settingKey + ".ValidationExpression");
            AdsRuleName = ruleName;
            MachineName = Environment.MachineName;
            _settingKey = settingKey;
            if (ruleDuration != null && ruleDuration.GetValueOrDefault(TimeSpan.Zero) > TimeSpan.Zero)
            {
                Duration = ruleDuration.Value;
            }
            else
            {
                Duration = TimeSpan.FromSeconds(Settings.GetInt32(settingKey + ".DurationInSeconds", 900));
            }
        }

        public void ReadSettings()
        {
            ValidationExpression = Settings.GetSetting(_settingKey + ".ValidationExpression");
            NegativeFilter = Settings.GetSetting(_settingKey + ".NegativeFilter");
        }
    }
}
