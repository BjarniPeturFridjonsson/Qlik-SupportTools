using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Eir.Common.Extensions
{
	public static class ConcurrentQueueExtensions
	{
		public static void Clear<T>(this ConcurrentQueue<T> queue)
		{
			if (queue == null) return;

			while (!queue.IsEmpty)
			{
				T item;
				queue.TryDequeue(out item);
			}
		}

		public static void MoveTo<T>(this ConcurrentQueue<T> source, ConcurrentQueue<T> target, int targetQueueSizeLimit = -1)
		{
			T item;
			while (source.TryDequeue(out item))
			{
				target.EnqueueWithLimit(item, targetQueueSizeLimit);
			}
		}

		public static void EnqueueWithLimit<T>(this ConcurrentQueue<T> queue, T item, int queueSizeLimit)
		{
			queue.Enqueue(item);

			while (queueSizeLimit > 0 && queue.Count > queueSizeLimit)
			{
				T throwawayItem;
				queue.TryDequeue(out throwawayItem);
			}
		}
		public static void EnqueueWithLimit<T>(this ConcurrentQueue<T> queue, IEnumerable<T> items, int queueSizeLimit)
		{
			foreach (var item in items)
			{
				queue.EnqueueWithLimit(item, queueSizeLimit);
			}
		}
	}
}