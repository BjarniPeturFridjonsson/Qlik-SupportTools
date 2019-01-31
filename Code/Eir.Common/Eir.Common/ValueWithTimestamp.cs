using System;

namespace Eir.Common
{
    public class ValueWithTimestamp<T>
    {
        public T Value { get; }
        public DateTime Timestamp { get; }

        public ValueWithTimestamp(T value, DateTime timestamp)
        {
            Value = value;
            Timestamp = timestamp;
        }
    }
}