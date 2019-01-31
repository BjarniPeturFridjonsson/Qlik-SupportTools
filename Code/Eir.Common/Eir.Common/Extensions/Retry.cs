using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Eir.Common.Extensions
{
	public static class Retry
	{
		/// <summary>
		/// Retry with exponential sleep between retries. 
		/// <para>Starts with trying, retrying on interval, retrying on Interval^2...</para>
		/// </summary>
		/// <param name="retryInterval"></param>
		/// <param name="maxRetries">how often to retry before throwing an exception</param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static void Do(Action action, TimeSpan retryInterval, int maxRetries = 3)
		{
			Do(new Func<object>(() =>
			{
				action();
				return null;
			}), retryInterval, maxRetries);
		}

		/// <summary>
		/// Retry with exponential sleep between retries. 
		/// <para>Starts with trying, retrying on Interval = Interval*retryCounter...</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="func"></param>
		/// <param name="interval"></param>
		/// <param name="maxRetries">how often to retry before throwing an exception</param>
		/// <returns></returns>
		public static T Do<T>(Func<T> func, TimeSpan interval, int maxRetries = 3)
		{
			var exs = new List<Exception>();
			int mSec = (int)interval.TotalMilliseconds;
			for (int retry = 0; retry < maxRetries; retry++)
			{
				try
				{
					if (retry > 0)
					{
						Thread.Sleep(mSec);
						mSec = mSec * retry;
					}

					return func();
				}
				catch (Exception ex)
				{
					exs.Add(ex);
				}
			}

			throw new AggregateException(exs);
		}

        public static async Task<T> Do<T>(Func<Task<T>> func, TimeSpan interval, int maxRetries = 3)
		{
			var exs = new List<Exception>();
			int mSec = (int)interval.TotalMilliseconds;
			for (int retry = 0; retry < maxRetries; retry++)
			{
				try
				{
					if (retry > 0)
					{
					    await Task.Delay(mSec);
						mSec = mSec * retry;
					}

					return await func();
				}
				catch (Exception ex)
				{
					exs.Add(ex);
				}
			}

			throw new AggregateException(exs);
		}

        public static async Task<T> Do<T>(Func<Task<T>> func, TimeSpan interval, CancellationToken ct, int maxRetries = 3)
		{
			var exs = new List<Exception>();
			int mSec = (int)interval.TotalMilliseconds;
			for (int retry = 0; retry < maxRetries; retry++)
			{
				try
				{
					if (retry > 0)
					{
					    await Task.Delay(mSec, ct);
						mSec = mSec * retry;
					}

					return await func();
				}
				catch (Exception ex)
				{
					exs.Add(ex);
				}
			}

			throw new AggregateException(exs);
		}
	}
}