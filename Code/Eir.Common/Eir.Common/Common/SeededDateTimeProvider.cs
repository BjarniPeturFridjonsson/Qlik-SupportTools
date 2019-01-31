using System;

namespace Eir.Common.Common
{
    /// <summary>
    /// <para>
    /// This <see cref="IDateTimeProvider"/> is intended for usages where
    /// the time sequence is fed by an external source, such as a datapoint
    /// stream, where we pick up the time from the CollectedTimeStamp of data
    /// points, and use it to "seed" this <see cref="IDateTimeProvider"/> with
    /// a time offset.   
    /// </para>
    /// <para>
    /// An example can be a host where the system clock is 47 seconds behind the
    /// system clock on which this code runs. Every time a datapoint is observed
    /// the code feeds the CollectedTimeStamp into this provider, which will note
    /// the time difference between this machine and the given time stamp, and
    /// using that difference to adjust the time returned by the <see cref="Time"/>
    /// method.
    /// </para>
    /// </summary>
    public class SeededDateTimeProvider : IDateTimeProvider
    {
        private readonly IDateTimeProvider _referenceTimeProvider;
        private DateTime _frozenTime;

        public SeededDateTimeProvider(IDateTimeProvider referenceTimeProvider)
        {
            _referenceTimeProvider = referenceTimeProvider;
        }

        public TimeSpan ReferenceDiff { get; private set; } = TimeSpan.Zero;

        /// <summary>
        /// Gets the current time for this time provider. The returned time will be the
        /// current time for the reference time provider, adjusted with the latest noted
        /// time difference (which is created by calling the <see cref="SetTimeSeed"/> method).
        /// </summary>
        public DateTime Time()
        {
            return _frozenTime == DateTime.MinValue
                ? _referenceTimeProvider.Time().Subtract(ReferenceDiff)
                : _frozenTime;
        }

        public DateTime Today => DateTime.Today;

        public DateTime MaxDateTime => _referenceTimeProvider.MaxDateTime;
        public DateTime MinDateTime => _referenceTimeProvider.MinDateTime;

        /// <summary>
        /// Adjusts the time difference between this time provider and the reference time. The time identified
        /// by <paramref name="currentTime"/> is compared to the current time of the reference time provider, and
        /// the difference is stored.
        /// </summary>
        public void SetTimeSeed(DateTime currentTime)
        {
            _frozenTime = DateTime.MinValue;
            ReferenceDiff = _referenceTimeProvider.Time().Subtract(currentTime);
        }

        /// <summary>
        /// Puts this <see cref="SeededDateTimeProvider"/> into a "frozen" state. All subsequent calls
        /// to <see cref="Time"/> will return the time that was current when this method was called. The
        /// <see cref="SeededDateTimeProvider"/> is automatically "unfrozen", resuming normal operation,
        /// when <see cref="SetTimeSeed"/> is called.
        /// </summary>
        public void FreezeTime()
        {
            _frozenTime = Time();
        }
    }
}