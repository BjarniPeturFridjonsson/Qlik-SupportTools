using System;
using System.Runtime.CompilerServices;

namespace Eir.Common.Logging
{
    public interface ITelemetry
    {
        void Add(Func<string> getMessage, [CallerFilePath] string filePath = null);
    }
}