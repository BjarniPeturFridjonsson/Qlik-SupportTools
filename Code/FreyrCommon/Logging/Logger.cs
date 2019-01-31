using System;

namespace FreyrCommon.Logging
{
    public class Logger : ILogger
    {
        private readonly string _name;

        public Logger(string name)
        {
            _name = name;
        }

        public void Add(string text)
        {
            Log.Add(text);
        }

        public void Add(string text, Exception exception)
        {
            Log.Add(text, exception);
        }

        public void Add(string text, LogLevel level)
        {
            Log.Add(text, level);
        }

        public void AddToEventLog(string text)
        {
            Log.AddToEventLog(_name, text);
        }

        public void AddToEventLog(Exception exception)
        {
            Log.AddToEventLog(_name, exception);
        }
    }
}