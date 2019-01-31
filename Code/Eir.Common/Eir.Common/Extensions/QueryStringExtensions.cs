using System;
using System.Collections.Specialized;

namespace Eir.Common.Extensions
{
    public static class QueryStringExtensions
    {
        public static Guid GetCustomerId(this NameValueCollection query)
        {
            return Guid.Parse(query["customerid"]);
        }

        public static DateTime GetFromDate(this NameValueCollection query)
        {
            return TimestampFromString(query["from"]);
        }

        public static DateTime GetToDate(this NameValueCollection query)
        {
            return TimestampFromString(query["to"]);
        }

        public static int GetLimit(this NameValueCollection query)
        {
            return LimitFromString(query["limit"]);
        }

        private static int LimitFromString(string value)
        {
            int result;
            return int.TryParse(value, out result) ? result : int.MaxValue;
        }

        private static DateTime TimestampFromString(string s)
        {
            if (!s.Contains("Z")) throw new Exception("wrong format");
            if (s.Length != 19) throw new Exception("wrong format");
            var parts = s.Split('Z');
            int year = int.Parse(parts[0].Substring(0, 4));
            int month = int.Parse(parts[0].Substring(5, 2));
            int day = int.Parse(parts[0].Substring(8, 2));

            int hour = int.Parse(parts[1].Substring(0, 2));
            int minute = int.Parse(parts[1].Substring(3, 2));
            int second = int.Parse(parts[1].Substring(6, 2));

            return new DateTime(year, month, day, hour, minute, second);
        }
    }
}