using System;
using System.Collections.Generic;

namespace Eir.Common.Collections.Adapting
{
    /// <summary>
    /// An adapting enumerables, i.e. an enumerable whose order will adapt based on iteration.
    /// </summary>
    public interface IAdaptingEnumerable<out T> : IEnumerable<T>
    {
        TimeSpan QuarantinePeriod { get; }
    }
}