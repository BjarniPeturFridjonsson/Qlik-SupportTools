using System;
using System.Runtime.CompilerServices;

namespace Eir.Common.Logging
{
    public interface IWindowsEventLog
    {
        void Info(string text);

        void Error(
            string text,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0);

        void Error(
            Exception exception,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0);

        void Error(
            string text,
            Exception exception,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0);
    }
}