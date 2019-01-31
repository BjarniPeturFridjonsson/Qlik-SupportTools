using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Eir.Common.Common;

namespace Eir.Common.Collections.Adapting
{
    /// <summary>
    /// This enumerable will pick items in random order.
    /// An item is put in quarantine for X seconds (temporarily removed from the set of items) if the next item is asked for (it's then regarded as "no good").
    /// </summary>
    public class RandomAdaptingEnumerable<T> : AdaptingEnumerable<T>
    {
        private class RandomEnumerator : IEnumerator<T>
        {
            private static readonly Random _random = new Random();

            private readonly ItemWrapper[] _originalItemWrappers;
            private readonly IDateTimeProvider _dateTimeProvider;
            private List<ItemWrapper> _itemWrappersLeft;
            private ItemWrapper _currentItemWrapper;

            public RandomEnumerator(ItemWrapper[] itemWrappers, IDateTimeProvider dateTimeProvider)
            {
                _originalItemWrappers = itemWrappers;
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
                    if (_itemWrappersLeft.Count == 0)
                    {
                        _currentItemWrapper = null;
                        return false;
                    }

                    int index = _itemWrappersLeft.Count == 1
                        ? 0
                        : _random.Next(_itemWrappersLeft.Count);

                    _currentItemWrapper = _itemWrappersLeft[index];

                    _itemWrappersLeft.RemoveAt(index);

                    if (!_currentItemWrapper.QuarantinedUntil.HasValue ||
                        _currentItemWrapper.QuarantinedUntil.Value <= _dateTimeProvider.Time())
                    {
                        return true;
                    }
                }
            }

            public void Reset()
            {
                _itemWrappersLeft = _originalItemWrappers.ToList();
            }

            public T Current => _currentItemWrapper.Item;

            object IEnumerator.Current => Current;
        }

        public RandomAdaptingEnumerable(IEnumerable<T> items, TimeSpan quarantinePeriod, IDateTimeProvider dateTimeProvider)
            : base(items, quarantinePeriod, dateTimeProvider)
        {
        }

        public override IEnumerator<T> GetEnumerator()
        {
            return new RandomEnumerator(ItemWrappers, DateTimeProvider);
        }
    }
}