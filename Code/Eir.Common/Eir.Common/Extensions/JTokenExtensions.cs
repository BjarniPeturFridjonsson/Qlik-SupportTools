using Newtonsoft.Json.Linq;

namespace Eir.Common.Extensions
{
	public static class JTokenExtensions
	{
		public static string AsString(this JToken token)
		{
			return token == null ? string.Empty : token.Value<string>() ?? string.Empty;
		}

		public static bool AsBoolean(this JToken token)
		{
			return token != null && token.Value<bool>();
		}

		public static int AsInt32(this JToken token)
		{
			return token == null ? 0 : token.Value<int>();
		}
	}
}