using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Eir.Common.Common;

namespace Eir.Common.Collections.Adapting
{
    /// <summary>
    /// Base class for adapting enumerables, i.e. an enumerable whose order will adapt based on iteration.
    /// </summary>
    public abstract class AdaptingEnumerable<T> : IAdaptingEnumerable<T>
    {
        protected readonly ItemWrapper[] ItemWrappers;

        protected class ItemWrapper
        {
            private readonly Func<DateTime> _getQuarantineUntil;
            private readonly Action _afterPlaceInQuarantine;

            public ItemWrapper(T item, Func<DateTime> getQuarantineUntil, Action afterPlaceInQuarantine)
            {
                _getQuarantineUntil = getQuarantineUntil;
                _afterPlaceInQuarantine = afterPlaceInQuarantine;
                Item = item;
            }

            public T Item { get; }

            public DateTime? QuarantinedUntil { get; private set; }

            public void PlaceInQuarantine()
            {
                QuarantinedUntil = _getQuarantineUntil();
                _afterPlaceInQuarantine();
            }

            public void RemoveFromQuarantine()
            {
                QuarantinedUntil = null;
            }
        }

        protected AdaptingEnumerable(IEnumerable<T> items, TimeSpan quarantinePeriod, IDateTimeProvider dateTimeProvider)
        {
            ItemWrappers = items
                .Select(item => new ItemWrapper(
                    item,
                    () => dateTimeProvider.Time().Add(quarantinePeriod),
                    QuarantineCleanUp))
                .ToArray();

            QuarantinePeriod = quarantinePeriod;
            DateTimeProvider = dateTimeProvider;
        }

        private void QuarantineCleanUp()
        {
            if (ItemWrappers.All(x => x.QuarantinedUntil.HasValue))
            {
                // If all are quarantined then revive the oldest one!
                DateTime oldestQuarantinedDateTime = ItemWrappers
                    .Where(x => x.QuarantinedUntil.HasValue)
                    .Min(x => x.QuarantinedUntil.Value);

                ItemWrapper oldestQuarantinedItem = ItemWrappers
                    .FirstOrDefault(x => x.QuarantinedUntil == oldestQuarantinedDateTime);

                oldestQuarantinedItem?.RemoveFromQuarantine();
            }
        }

        public TimeSpan QuarantinePeriod { get; }

        protected IDateTimeProvider DateTimeProvider { get; }

        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}