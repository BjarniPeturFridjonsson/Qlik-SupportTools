using System;
using System.Collections.Generic;
using Eir.Common.IO;
using Gjallarhorn.Db;

namespace Gjallarhorn.Notifiers
{
    public class DbNotifyerDeamon : INotifyerDaemon
    {
        private readonly List<string> _obervedMonitors = new List<string>();
        private readonly GjallarhornDb _gjallarhornDb;
        public Boolean SendRawMessage() => true;
        public String GetBodyTemplate() => "";

        public DbNotifyerDeamon()
        {
            _gjallarhornDb = new GjallarhornDb(FileSystem.Singleton);
        }

        public void EnqueueMessage(string monitorName, string text)
        {
            if (!_obervedMonitors.Contains(monitorName))
            {
                _gjallarhornDb.EnsureMonitorTableExists(monitorName);
              _obervedMonitors.Add(monitorName);
            }

            _gjallarhornDb.SaveMonitorData(monitorName, text);
        }
    }
}
