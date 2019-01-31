using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eir.Common.IO;
using Eir.Common.Time;

namespace Eir.Common.Logging
{
    public interface IDynamicLog<T> where T:LogItem
    {
        
        void Add(T logObject);
        void Dispose();
    }

    public class DynamicLogItem : LogItem
    {
        

        public DynamicLogItem(string[] cols):base(cols)
        {
            
        }

       
        //public DynamicLogItem CreateHeaders()
        //{
        //    var newA = new[] { timestamp.ToString("O"), logLevel.ToString() };
        //    Array.Copy(values, newA, values.Length);
        //    return newA;
        //}

        public override DateTime Timestamp { get; }

        public override LogLevel LogLevel { get; }
    }
    
    public class DynamicLog<T> : IDynamicLog<T>, IDisposable where T: LogItem
    {
        private readonly FileWriterLogItemHandler<T> _fileWriterLogItemHandler;
        private readonly QueuedLogItemHandler<T> _queuedLogItemHandler;
        private readonly ITrigger _trigger;

        public DynamicLog(string logDir, LogFileNameComposer logFileNameComposer, IFileSystem fileSystem, T headersList)
        {
            _fileWriterLogItemHandler = new FileWriterLogItemHandler<T>(logDir, logFileNameComposer, fileSystem, headersList);

            _trigger = new PauseTrigger(() => TimeSpan.FromSeconds(1));

            _queuedLogItemHandler = new QueuedLogItemHandler<T>(_fileWriterLogItemHandler, _trigger);

        }

        public void Add(T logObject)
        {
            _queuedLogItemHandler.Add(logObject);
        }
        
        public void Dispose()
        {
            _trigger.Dispose();
            _queuedLogItemHandler.Dispose();
            _fileWriterLogItemHandler.Dispose();
        }
    }
}
