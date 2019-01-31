using System;

namespace Eir.Common.Logging
{
    public static class Log
    {
        public const string DEFAULT_DIR_NAME = "Log";

        static Log()
        {
            To = NullLogs.Instance;
        }

        public static ILogs To { get; private set; }

        public static void Init(ILogs logs)
        {
            if ((To != null) && (To != NullLogs.Instance))
            {
                throw new Exception("The log is already initiated!");
            }

            To = logs ?? NullLogs.Instance;
        }

        public static void Shutdown()
        {
            ILogs logs = To;
            To = NullLogs.Instance;

            logs.Dispose();
        }

        public static class Testing
        {
            public static ILogs ExchangeInstance(ILogs logs)
            {
                ILogs oldLogs = To;
                To = logs ?? NullLogs.Instance;
                return oldLogs;
            }
        }
    }
}