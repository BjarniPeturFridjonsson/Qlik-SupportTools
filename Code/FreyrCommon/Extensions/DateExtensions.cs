using System;
using System.Globalization;

namespace FreyrCommon.Extensions
{
    public static class DateExtensions
    {
        private const string DATAPOINT_FORMAT = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";
        private const string SETTINGS_FORMAT = "yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fff";

        public static string Format()
        {
            return "yyyy'-'MM'-'dd HH':'mm':'ss";
        }

        public static string AsDatapointString(this DateTime dateTime)
        {
            return dateTime.ToString(DATAPOINT_FORMAT);
        }

        public static string AsSettingsString(this DateTime dateTime)
        {
            return dateTime.ToString(SETTINGS_FORMAT);
        }

        public static DateTime FromSettingsString(string settingsDateTime, DateTime defaultValue)
        {
            DateTime dateTime;
            return DateTime.TryParseExact(settingsDateTime, SETTINGS_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dateTime)
                ? dateTime
                : defaultValue;
        }

        public static DateTime TruncateMilliseconds(this DateTime dateTime)
        {
            return dateTime.AddMilliseconds(-dateTime.Millisecond);
        }
    }
}
