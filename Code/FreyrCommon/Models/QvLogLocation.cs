using System;
using System.Threading;
using Newtonsoft.Json;

namespace FreyrCommon.Models
{
    public enum QvLogLocationSource
    {
        QdsClusterInfoUrl,QvsSetting, QdsSettingsApplicationDataFolder
    }

    public enum QvLogCollectionType
    {
        All,SettingsOnly
    }

    public class QvLogLocation
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public QvLogLocationSource Type { get; set; }
        public QvLogCollectionType LogCollectionType { get; set; } = QvLogCollectionType.All;

        [JsonIgnore]
        public Func<string, bool> IgnorePaths { get; set; }

        private long _fileCount = 0;

        public long GetFileCount()
        {
            return _fileCount;
        }

        public void AddToFileCount(long count)
        {
            Interlocked.Add(ref _fileCount, count);
        }
    }
}
