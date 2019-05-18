using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eir.Common.IO;
using Gjallarhorn.Db;

namespace Gjallarhorn.QvLogReading
{
    public class QvLogHelper
    {
        private readonly IFileSystem _fileSystem = FileSystem.Singleton;
        private readonly SettingsDb _settingsDb = new SettingsDb(FileSystem.Singleton);

        public QvLogHelper()
        {
            _settingsDb.EnsureSettingsTableExists();
        }

        public void SessionLogFileSetting(string value) => SaveSettingBase("SessionLogCurrentFilePath", value);
        public FileSetting SessionLogFileSetting()
        {
            var s = GetSettingsString("SessionLogCurrentFilePath");
            return string.IsNullOrWhiteSpace(s) ? null : 
                _fileSystem.FileExists(s) ? new FileSetting(s) : null;
        }


        public long SessionLogPostionSetting() => GetSettingLong("SessionLogReadPostion");
        public void SessionLogPostionSetting(long value) => SaveSettingBase("SessionLogReadPostion",value.ToString());

        private string GetSettingsString(string key) => ReadSettingBase(key);

        private long GetSettingLong(string key)
        {
            var s = ReadSettingBase(key);
            if (string.IsNullOrWhiteSpace(s)) return 0;
            return !long.TryParse(s, out var ret) ? 0 : ret;
        }

        private string ReadSettingBase(string key)
        {
            return _settingsDb.ReadSettings($"QlikViewLogFileParserMonitor.{key}");
        }
        private void SaveSettingBase(string key, string value)
        {
            _settingsDb.SaveSettings($"QlikViewLogFileParserMonitor.{key}", value);
        }

    }
}
