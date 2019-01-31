using System;
using System.Text;

namespace Eir.Common.Net
{
    public static class UriSupport
    {
        public static string Combine(string baseUri, params UriArg[] args)
        {
            return Combine(baseUri, string.Empty, args);
        }

        public static string Combine(string baseUri, UriFragment uriFragment)
        {
            return uriFragment == null
                ? baseUri
                : Combine(baseUri, uriFragment.ApiEndpointFragment, uriFragment.Args);
        }

        public static string Combine(string baseUri, string apiEndpointFragment, params UriArg[] args)
        {
            if (string.IsNullOrEmpty(baseUri))
            {
                throw new ArgumentException($"{nameof(baseUri)} must not be null or empty");
            }

            var uri = new StringBuilder(baseUri);

            if (apiEndpointFragment == null)
            {
                apiEndpointFragment = string.Empty;
            }

            if (!string.IsNullOrEmpty(apiEndpointFragment))
            {
                bool firstPartEndsWithSlash = baseUri.EndsWith("/");
                bool secondPartStartsWithSlash = apiEndpointFragment.StartsWith("/");

                if (firstPartEndsWithSlash)
                {
                    uri.Append(secondPartStartsWithSlash
                        ? apiEndpointFragment.Substring(1)
                        : apiEndpointFragment);
                }
                else
                {
                    if (!secondPartStartsWithSlash)
                    {
                        uri.Append('/');
                    }

                    uri.Append(apiEndpointFragment);
                }
            }

            if (args.Length > 0)
            {
                if (!baseUri.Contains("?") && !apiEndpointFragment.Contains("?"))
                {
                    uri.Append('?');
                }
                else if (!baseUri.EndsWith("?") && !apiEndpointFragment.EndsWith("?") &&
                    !baseUri.EndsWith("&") && !apiEndpointFragment.EndsWith("&"))
                {
                    uri.Append('&');
                }

                for (int i = 0; i < args.Length; i++)
                {
                    if (i > 0)
                    {
                        uri.Append('&');
                    }

                    uri.Append(Uri.EscapeDataString(args[i].Name));
                    uri.Append('=');
                    uri.Append(Uri.EscapeDataString(args[i].Value?.ToString() ?? "null"));
                }
            }

            return uri.ToString();
        }
    }

    public class UriArg
    {
        public UriArg(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public object Value { get; }
    }
}