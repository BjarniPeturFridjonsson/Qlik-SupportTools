using System;
using System.Collections.Generic;
using System.Linq;
using Eir.Common.Collections.Adapting;
using Eir.Common.Common;

namespace Eir.Common.Net
{
    public class MultiUriSelectionStrategyFactory : IMultiUriSelectionStrategyFactory
    {
        private readonly MultiUriSelectionStrategy _strategy;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly TimeSpan _quarantinePeriod;

        public static readonly IMultiUriSelectionStrategyFactory Default = new MultiUriSelectionStrategyFactory(
            MultiUriSelectionStrategy.LastSuccessful,
            DateTimeProvider.Singleton,
            TimeSpan.FromSeconds(10));

        public MultiUriSelectionStrategyFactory(
            MultiUriSelectionStrategy strategy,
            IDateTimeProvider dateTimeProvider,
            TimeSpan quarantinePeriod)
        {
            _strategy = strategy;
            _dateTimeProvider = dateTimeProvider;
            _quarantinePeriod = quarantinePeriod;
        }

        public IAdaptingEnumerable<Uri> GetAdaptingEnumerable(IEnumerable<Uri> uris)
        {
            Uri[] uriArray = uris.ToArray();

            switch (_strategy)
            {
                case MultiUriSelectionStrategy.LastSuccessful:
                    return new LastSuccessfulAdaptingEnumerable<Uri>(uriArray, _quarantinePeriod, _dateTimeProvider);

                case MultiUriSelectionStrategy.RoundRobin:
                    return new RoundRobinAdaptingEnumerable<Uri>(uriArray, _quarantinePeriod, _dateTimeProvider);

                case MultiUriSelectionStrategy.Random:
                    return new RandomAdaptingEnumerable<Uri>(uriArray, _quarantinePeriod, _dateTimeProvider);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}