namespace Gjallarhorn.Program
{
    public class ObserverData
    {
        public long Count { get; private set; }
        private long RunningSum { get; set; }
        public long ObservedMaxValue { get; private set; }
        public long ObservedMinValue { get; private set; } = int.MinValue;

        public void Add(long seconds)
        {
            Count++;
            RunningSum += seconds;
            if (seconds > ObservedMaxValue)
                ObservedMaxValue = seconds;
            if (seconds < ObservedMinValue || ObservedMinValue == int.MinValue)
                ObservedMinValue = seconds;
        }

        public int CurrentAvg()
        {
            if (Count == 0)
                return -1;
            return (int)(RunningSum / Count);
        }
    }
}
