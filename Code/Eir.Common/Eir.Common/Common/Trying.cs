using System;

namespace Eir.Common.Common
{
    public class Trying
    {
        public static readonly Trying Once = new Trying(1, TimeSpan.Zero);

        public static readonly Trying ThreeTimes = new Trying(3, TimeSpan.FromSeconds(1));

        public Trying(int count, TimeSpan pause)
        {
            Count = count;
            Pause = pause;
        }

        public int Count { get; }

        public TimeSpan Pause { get; }
    }
}