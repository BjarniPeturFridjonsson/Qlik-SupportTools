using System;

namespace Eir.Common.Logging
{
    public class MainLogItemEventArgs : EventArgs
    {
        public MainLogItemEventArgs(MainLogItem logItem)
        {
            LogItem = logItem;
        }

        public MainLogItem LogItem { get; }
    }
}