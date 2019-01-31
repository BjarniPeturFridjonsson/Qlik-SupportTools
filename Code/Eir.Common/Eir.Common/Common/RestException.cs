using System;
using System.Collections.Generic;
using Eir.Common.Rest;

namespace Eir.Common.Common
{
    public class RestException : Exception
    {
        public IEnumerable<RestError> Errors { get; }

        public RestException(IEnumerable<RestError> errors)
        {
            Errors = errors;
        }
    }
}