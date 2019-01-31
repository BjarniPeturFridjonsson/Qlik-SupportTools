using System.Diagnostics;
using System.Threading;

namespace Eir.Common.Diagnostics
{
    public class PerformanceMonitorValue
    {
        private readonly PerformanceCounter _performanceCounter;
        // ReSharper disable once NotAccessedField.Local
        private readonly string _key; // to be used for debugging purposes
        private long _currentValue;

        public PerformanceMonitorValue(PerformanceCounter performanceCounter, string key, bool keepExistingCounterValue)
        {
            _performanceCounter = performanceCounter;
            _key = key;
            if (!keepExistingCounterValue)
            {
                _performanceCounter.RawValue = 0;
            }
        }

        public long Increment()
        {
            return IncrementBy(1);
        }

        public long IncrementBy(long delta)
        {
            _performanceCounter.IncrementBy(delta);
            var result = Interlocked.Add(ref _currentValue, delta);
            return result;
        }

        public long Decrement()
        {
            return DecrementBy(1);
        }

        public long DecrementBy(long delta)
        {
            return IncrementBy(-delta);
        }

        public long Value => _currentValue;
    }
}