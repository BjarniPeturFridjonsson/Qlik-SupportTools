using System;
using System.Threading;
using System.Threading.Tasks;
using BrightIdeasSoftware;
using Eir.Common.Logging;

namespace FreyrViewer.Ui.Grids
{
    internal interface ILoaderWithTimer
    {
        TimeSpan? ReloadInterval { get; }

        void SetReloadPeriodically(TimeSpan reloadInterval);

        void SetReloadOnce();

         void Manualreload(object loader);
    }

   
    internal class LoaderWithTimer<T> : ILoaderWithTimer
    {
        private readonly Timer _timer;
        private Func<Task<T>> _loader;
        private readonly Action<T> _onAfterLoad;
        private readonly Action<Exception> _onException;
        private bool _disposed;

        public LoaderWithTimer(
            FastDataListView grid,
            Func<Task<T>> loader,
            Action<T> onAfterLoad,
            Action<Exception> onException)
        {
            _loader = loader;
            _onAfterLoad = onAfterLoad;
            _onException = onException;

            _timer = new Timer(async t => await Tick());

            EventHandler gridOnDisposed = null;

            grid.Disposed += gridOnDisposed = (s, e) =>
            {
                _disposed = true;
                _timer.Dispose();
                grid.Disposed -= gridOnDisposed;
            };


        }

        public void Manualreload(object loader)
        {
            var load = loader as Func<Task<T>>;
            _loader = load;
            SetReloadOnce();
        }
        public TimeSpan? ReloadInterval { get; private set; }

        public void SetReloadPeriodically(TimeSpan reloadInterval)
        {
            ReloadInterval = reloadInterval;
            _timer.Change(0, Timeout.Infinite);
        }

        public void SetReloadOnce()
        {
            ReloadInterval = null;
            _timer.Change(0, Timeout.Infinite);
        }

        private async Task Tick()
        {
            try
            {
                if (_disposed)
                {
                    return;
                }

                T data = await _loader();

                if (_disposed)
                {
                    return;
                }

                _onAfterLoad(data);
            }
            catch (Exception ex)
            {
                Log.To.Main.Add($"LoaderWithTimer<{typeof(T)}> failed Tick with:{ex}");

                try
                {
                    _onException(ex);
                }
                catch
                {
                }
            }
            finally
            {
                try
                {
                    if (!_disposed && ReloadInterval.HasValue)
                    {
                        _timer.Change((long)ReloadInterval.Value.TotalMilliseconds, Timeout.Infinite);
                    }
                }
                catch
                {
                }
            }
        }
    }
}