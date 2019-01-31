using System;

namespace Eir.Common.Net
{
    public class UriFragment
    {
        public UriFragment(string apiEndpointFragment, params UriArg[] args)
        {
            ApiEndpointFragment = apiEndpointFragment;
            Args = args;
        }

        public UriFragment(params UriArg[] args)
        {
            ApiEndpointFragment = string.Empty;
            Args = args;
        }

        public string ApiEndpointFragment { get; }

        public UriArg[] Args { get; }

        public UriFragment Append(string apiEndpointFragment, params UriArg[] args)
        {
            if (Args.Length > 0)
            {
                throw new ArgumentException("You may not append another API endpoint fragment if the first one has arguments!");
            }

            string combinedFragments = UriSupport.Combine(ApiEndpointFragment, apiEndpointFragment);

            return new UriFragment(combinedFragments, args);
        }

        public UriFragment Append(params UriArg[] args)
        {
            if (Args.Length > 0)
            {
                throw new ArgumentException("You may not append another API endpoint fragment if the first one has arguments!");
            }

            return new UriFragment(ApiEndpointFragment, args);
        }

        public override string ToString()
        {
            return UriSupport.Combine("...", ApiEndpointFragment, Args);
        }
    }
}