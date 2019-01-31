using System;
using System.Runtime.CompilerServices;

namespace Eir.Common.Logging
{
    public interface IMainLog
    {
        event EventHandler<MainLogItemEventArgs> NewLogItem;

        void Add(
            string message,
            LogLevel logLevel = LogLevel.Info,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0);

        void AddException(
            string message,
            Exception exception,
            LogLevel logLevel = LogLevel.Error,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0);
    }
}