using System;

namespace Eir.Common.Common
{
    public class CacheItem<T>
    {
        public CacheItem(T item, DateTime createdTime)
        {
            Item = item;
            CreatedTime = createdTime;
        }

        public T Item { get; private set; }
        public DateTime CreatedTime { get; private set; }
    }
}