using System;
using System.Threading;
using Eir.Common.Logging;

namespace Eir.Common.Time
{
    /// <summary>
    /// This trigger invokes the delegates on an interval, but it pauses while the delegate executes:
    /// [wait X seconds]         [wait X seconds]         [...]
    ///                 [execute]                [execute]
    /// </summary>
    public class PauseTrigger : TriggerBase
    {
        private readonly Func<TimeSpan> _intervalProvider;
        private readonly Timer _timer;
        private static readonly TimeSpan _disabled = TimeSpan.FromMilliseconds(-1);

        public PauseTrigger(Func<TimeSpan> intervalProvider, TimeSpan? initialPause = null)
        {
            _intervalProvider = intervalProvider;

            Log.To.Main.Add("Initiating PauseTrigger.");
            _timer = new Timer(state =>
            {
                // disable the timer while the method executes...
                _timer.Change(_disabled, _disabled);

                TimeSpan interval;
                if (ShouldExecute(out interval))
                {
                    // Invoke the trigger, and then re-enable the timer.
                    TrigInternal().ContinueWith(t => _timer.Change(interval, interval));
                }
            });

            TimeSpan initialInterval = GetInitialInterval(initialPause);

            _timer.Change(initialInterval, initialInterval);
        }

        private TimeSpan GetInitialInterval(TimeSpan? initialPause)
        {
            TimeSpan initialInterval;
            if (initialPause.HasValue)
            {
                initialInterval = initialPause.Value;
                if (initialInterval <= TimeSpan.Zero)
                {
                    initialInterval = TimeSpan.FromSeconds(3);
                }
            }
            else
            {
                ShouldExecute(out initialInterval);
            }

            return initialInterval;
        }

        private bool ShouldExecute(out TimeSpan pause)
        {
            pause = _intervalProvider();
            if (pause > TimeSpan.Zero)
            {
                return true;
            }

            // Disabled. Wait for a few seconds and then try again.
            pause = TimeSpan.FromSeconds(3);
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            // Dispose the timer before calling base.Dispose, since
            // base dispose might take a while if there are running 
            // tasks. We don't want any race conditions here, where 
            // the timer will get a chance to trigger between tasks 
            // being completed and Dispose returning here.
            if (disposing)
            {
                _timer?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}