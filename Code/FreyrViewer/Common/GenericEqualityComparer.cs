using System;
using System.Collections.Generic;

namespace FreyrViewer.Common
{
    public class GenericEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, Guid> _getId;

        public GenericEqualityComparer(Func<T, Guid> getId)
        {
            _getId = getId;
        }

        public bool Equals(T x, T y)
        {
            return _getId(x) == _getId(y);
        }

        public int GetHashCode(T obj)
        {
            return _getId(obj).GetHashCode();
        }
    }
}