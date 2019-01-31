using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eir.Common.Time;

namespace Eir.Common.Logging
{
    public class QueuedLogItemHandler<TLogItem> : ILogItemHandler<TLogItem>
        where TLogItem : LogItem
    {
        private readonly object _syncObj = new object();
        private readonly ILogItemHandler<TLogItem> _nestedLogItemHandler;
        private readonly ITrigger _trigger;
        private readonly Queue<TLogItem> _logBuffer;

        public QueuedLogItemHandler(ILogItemHandler<TLogItem> nestedLogItemHandler, ITrigger trigger)
        {
            _nestedLogItemHandler = nestedLogItemHandler;
            _trigger = trigger;

            _logBuffer = new Queue<TLogItem>();

            _trigger.RegisterAction(HandleQueue);
        }

        public void Dispose()
        {
            _trigger.UnregisterAction(HandleQueue);
        }

        public void Add(TLogItem logItem)
        {
            lock (_syncObj)
            {
                _logBuffer.Enqueue(logItem);
            }
        }

        private Task HandleQueue()
        {
            try
            {
                while (true)
                {
                    TLogItem logItem;
                    if (!TryDequeue(out logItem))
                    {
                        break;
                    }

                    _nestedLogItemHandler.Add(logItem);
                }
            }
            catch (Exception ex)
            {
                Log.To.WindowsEvent.Error($"{nameof(QueuedLogItemHandler<TLogItem>)}.{nameof(HandleQueue)}", ex);
            }

            return Task.FromResult(false);
        }

        private bool TryDequeue(out TLogItem logItem)
        {
            lock (_syncObj)
            {
                if (_logBuffer.Count == 0)
                {
                    logItem = default(TLogItem);
                    return false;
                }

                logItem = _logBuffer.Dequeue();
                return true;
            }
        }
    }
}