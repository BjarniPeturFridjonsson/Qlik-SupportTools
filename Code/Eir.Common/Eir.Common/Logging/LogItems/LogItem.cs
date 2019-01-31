using System;

namespace Eir.Common.Logging
{
    public abstract class LogItem
    {
        protected LogItem(params string[] values)
        {
            Values = values;
        }

        public abstract DateTime Timestamp { get; }

        public abstract LogLevel LogLevel { get; }

        public string[] Values { get; }
    }
}