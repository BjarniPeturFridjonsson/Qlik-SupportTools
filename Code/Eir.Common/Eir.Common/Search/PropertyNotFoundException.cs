using System;

namespace Eir.Common.Search
{
    internal class PropertyNotFoundException : Exception
    {
        public PropertyNotFoundException(string message) : base(message)
        {
        }
    }
}