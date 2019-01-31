using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Eir.Common.Logging;

namespace Eir.Common.Collections
{
    public class NotifyingConcurrentQueue<T>
    {
        private readonly ConcurrentQueue<T> _queue;

        public AutoResetEvent Waiter { get; private set; }

        public bool IsEmpty
        {
            get { return _queue.IsEmpty; }
        }

        public NotifyingConcurrentQueue()
        {
            Waiter = new AutoResetEvent(false);
            _queue = new ConcurrentQueue<T>();
        }

        public void EnqueueMany(List<T> items)
        {
            
            if (items.Count > 0)
            {
                items.ForEach(_queue.Enqueue);
                Waiter.Set();
            }
            
        }

        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
            Waiter.Set();
        }

        public bool TryDequeue(out T result)
        {
            return _queue.TryDequeue(out result);
        }

        public bool TryDequeueMany(out List<T> result, int limit)
        {
            try
            {
                result = new List<T>(limit);
                while (--limit >= 0)
                {
                    T item;
                    if (_queue.TryDequeue(out item))
                        result.Add(item);
                    else break;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.To.Main.AddException("Failed to dequeue items", ex);
                result = new List<T>(0);
                return false;
            }
        }
    }
}