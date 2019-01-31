using System;

namespace FreyrCommon.Logging
{
    public interface ILogger
    {
        void Add(string text);

        void Add(string text, Exception exception);

        void Add(string text, LogLevel level);

        void AddToEventLog(string text);

        void AddToEventLog(Exception exception);
    }
}
