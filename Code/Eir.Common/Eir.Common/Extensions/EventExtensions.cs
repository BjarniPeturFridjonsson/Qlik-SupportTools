using System;

namespace Eir.Common.Extensions
{
	public static class EventExtensions
	{
		public static void SafeInvoke(this EventHandler evt, object sender, EventArgs e)
		{
		    evt?.Invoke(sender, e);
		}

		public static void SafeInvoke<T>(this EventHandler<T> evt, object sender, T e) where T : EventArgs
		{
		    evt?.Invoke(sender, e);
		}
	}
}
