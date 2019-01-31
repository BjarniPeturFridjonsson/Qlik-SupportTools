using System;
using System.Collections.Generic;
using Eir.Common.Collections.Adapting;

namespace Eir.Common.Net
{
    public interface IMultiUriSelectionStrategyFactory
    {
        IAdaptingEnumerable<Uri> GetAdaptingEnumerable(IEnumerable<Uri> uris);
    }
}