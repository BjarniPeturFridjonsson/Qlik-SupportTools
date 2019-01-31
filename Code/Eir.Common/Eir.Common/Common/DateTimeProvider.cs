using System;
using System.Data.SqlTypes;

namespace Eir.Common.Common
{
    public interface IDateTimeProvider
    {
        DateTime Time();
        DateTime Today { get; }
        DateTime MaxDateTime { get; }
        DateTime MinDateTime { get; }
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        //accessing datetime directly should get your fingers slapped.. 
        //accessing datetime directly should get your fingers slapped.. 
        //accessing datetime directly should get your fingers slapped.. 
        //accessing datetime directly should get your fingers slapped.. 

        private static readonly DateTime _maxDateTime = new DateTime(9999, 12, 31, 23, 59, 59);

        public static IDateTimeProvider Singleton => new DateTimeProvider();

        private DateTimeProvider()
        {
        }

        public DateTime Time()
        {
            return DateTime.UtcNow;
        }

        public DateTime Today => DateTime.Now.Date;
        public DateTime MaxDateTime => _maxDateTime;
        public DateTime MinDateTime => SqlDateTime.MinValue.Value;
    }
}