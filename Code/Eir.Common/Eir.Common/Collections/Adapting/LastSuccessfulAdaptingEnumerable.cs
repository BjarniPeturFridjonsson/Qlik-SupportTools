using System;
using System.Collections;
using System.Collections.Generic;
using Eir.Common.Common;

namespace Eir.Common.Collections.Adapting
{
    /// <summary>
    /// This enumerable will restart on the same item where its previous iteration were aborted (i.e. "accepted").
    /// An item is put in quarantine for X seconds (temporarily removed from the set of items) if the next item is asked for (it's then regarded as "no good").
    /// </summary>
    public class LastSuccessfulAdaptingEnumerable<T> : AdaptingEnumerable<T>
    {
        private class LastSuccessfulEnumerator : IEnumerator<T>
        {
            private readonly ItemWrapper[] _itemsWrappers;
            private readonly IDateTimeProvider _dateTimeProvider;
            private int _currentIndex;
            private ItemWrapper _currentItemWrapper;

            public LastSuccessfulEnumerator(
                ItemWrapper[] itemWrappers,
                IDateTimeProvider dateTimeProvider)
            {
                _itemsWrappers = itemWrappers;
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
                    _currentIndex++;

                    if (_currentIndex >= _itemsWrappers.Length)
                    {
                        _currentItemWrapper = null;
                        return false;
                    }

                    _currentItemWrapper = _itemsWrappers[_currentIndex];

                    if (!_currentItemWrapper.QuarantinedUntil.HasValue ||
                        _currentItemWrapper.QuarantinedUntil.Value <= _dateTimeProvider.Time())
                    {
                        return true;
                    }
                }
            }

            public void Reset()
            {
                _currentIndex = -1;
            }

            public T Current => _currentItemWrapper.Item;

            object IEnumerator.Current => Current;
        }

        public LastSuccessfulAdaptingEnumerable(IEnumerable<T> items, TimeSpan quarantinePeriod, IDateTimeProvider dateTimeProvider)
            : base(items, quarantinePeriod, dateTimeProvider)
        {
        }

        public override IEnumerator<T> GetEnumerator()
        {
            return new LastSuccessfulEnumerator(ItemWrappers, DateTimeProvider);
        }
    }
}