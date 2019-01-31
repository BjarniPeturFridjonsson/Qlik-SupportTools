using System;

namespace Eir.Common.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string AsReadableString(this TimeSpan ts)
        {
            if (ts.Days > 0)
            {
                var hourString = GetHourString(ts);
                return $"{GetDayString(ts)}{(hourString.StartsWith("0 ") ? string.Empty : $" and {hourString}")}";
            }

            if (ts.Hours > 0)
            {
                var minuteString = GetMinuteString(ts);
                return $"{GetHourString(ts)}{(minuteString.StartsWith("0 ") ? string.Empty : $" and {minuteString}")}";
            }

            if (ts.Minutes > 0)
            {
                var secondString = GetSecondString(ts);
                return $"{GetMinuteString(ts)}{(secondString.StartsWith("0 ") ? string.Empty : $" and {secondString}")}";
            }

            return GetSecondString(ts);
        }

        private static string GetDayString(TimeSpan ts)
        {
            return GetStringWithPluralization(ts.Days, "day");
        }

        private static string GetHourString(TimeSpan ts)
        {
            return GetStringWithPluralization(ts.Hours, "hour");
        }

        private static string GetMinuteString(TimeSpan ts)
        {
            return GetStringWithPluralization(ts.Minutes, "minute");
        }

        private static string GetSecondString(TimeSpan ts)
        {
            return GetStringWithPluralization(ts.Seconds, "second");
        }

        private static string GetStringWithPluralization(int number, string wordbase)
        {
            return string.Format("{0} {1}{2}", number, wordbase, number != 1 ? "s" : string.Empty);
        }
    }
}