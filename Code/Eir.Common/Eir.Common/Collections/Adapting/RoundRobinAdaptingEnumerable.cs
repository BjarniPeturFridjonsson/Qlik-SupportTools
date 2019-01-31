using System;
using System.Collections;
using System.Collections.Generic;
using Eir.Common.Common;

namespace Eir.Common.Collections.Adapting
{
    /// <summary>
    /// This enumerable uses a "global" enumerator; so multiple (also simultaneously cross-thread) iterations will get the next item in line.
    /// An item is put in quarantine for X seconds (temporarily removed from the set of items) if the next item is asked for (it's then regarded as "no good").
    /// </summary>
    public class RoundRobinAdaptingEnumerable<T> : AdaptingEnumerable<T>
    {
        private readonly object _syncObj = new object();
        private int _currentIndex = -1;

        private class RoundRobinEnumerator : IEnumerator<T>
        {
            private readonly ItemWrapper[] _itemsWrappers;
            private readonly Func<ItemWrapper> _getNextItem;
            private readonly IDateTimeProvider _dateTimeProvider;
            private readonly HashSet<ItemWrapper> _testedItemWrappers = new HashSet<ItemWrapper>();
            private ItemWrapper _currentItemWrapper;

            public RoundRobinEnumerator(
                ItemWrapper[] itemWrappers,
                Func<ItemWrapper> getNextItem,
                IDateTimeProvider dateTimeProvider)
            {
                _itemsWrappers = itemWrappers;
                _getNextItem = getNextItem;
                _dateTimeProvider = dateTimeProvider;
                Reset();
            }

            public void Dispose()
            {
                _currentItemWrapper?.RemoveFromQuarantine();
            }

            public bool MoveNext()
            {
                _currentItemWrapper?.PlaceInQuarantine();

                while (true)
                {
                    if (_testedItemWrappers.Count == _itemsWrappers.Length)
                    {
                        _currentItemWrapper = null;
                        return false;
                    }

                    _currentItemWrapper = _getNextItem();
                    _testedItemWrappers.Add(_currentItemWrapper);

                    if (!_currentItemWrapper.QuarantinedUntil.HasValue ||
                        _currentItemWrapper.QuarantinedUntil.Value <= _dateTimeProvider.Time())
                    {
                        return true;
                    }
                }
            }

            public void Reset()
            {
                _testedItemWrappers.Clear();
            }

            public T Current => _currentItemWrapper.Item;

            object IEnumerator.Current => Current;
        }

        public RoundRobinAdaptingEnumerable(IEnumerable<T> items, TimeSpan quarantinePeriod, IDateTimeProvider dateTimeProvider)
            : base(items, quarantinePeriod, dateTimeProvider)
        {
        }

        public override IEnumerator<T> GetEnumerator()
        {
            return new RoundRobinEnumerator(
                ItemWrappers,
                GetNextItemWrapper,
                DateTimeProvider);
        }

        private ItemWrapper GetNextItemWrapper()
        {
            lock (_syncObj)
            {
                _currentIndex = (_currentIndex + 1) % ItemWrappers.Length;
                return ItemWrappers[_currentIndex];
            }
        }
    }
}