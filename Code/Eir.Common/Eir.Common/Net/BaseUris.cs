using System;
using System.Collections.Generic;
using System.Linq;
using Eir.Common.Collections.Adapting;

namespace Eir.Common.Net
{
    public class BaseUris
    {
        private readonly Uri[] _uriArray;

        public BaseUris(IEnumerable<string> uris, IMultiUriSelectionStrategyFactory multiUriSelectionStrategyFactory)
            : this(
                 uris.Select(x => x?.Trim()).Where(x => x != null).Select(x => new Uri(x)).ToArray(),
                 multiUriSelectionStrategyFactory)
        {
        }

        public BaseUris(IEnumerable<Uri> uris, IMultiUriSelectionStrategyFactory multiUriSelectionStrategyFactory)
        {
            _uriArray = uris.Where(x => x != null).Distinct(EqualityComparer<Uri>.Default).ToArray();

            if (_uriArray.Length == 0)
            {
                throw new ArgumentException("At least one URI must be supplied!");
            }

            Uris = multiUriSelectionStrategyFactory.GetAdaptingEnumerable(_uriArray);
        }

        public IAdaptingEnumerable<Uri> Uris { get; }

        public IEnumerable<Uri> GetConfiguredUris() => _uriArray;
        
    }
}