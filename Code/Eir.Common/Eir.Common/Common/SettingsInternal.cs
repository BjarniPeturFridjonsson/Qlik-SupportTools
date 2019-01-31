using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Eir.Common.IO;
using Eir.Common.Logging;

namespace Eir.Common.Common
{
    public class SettingsInternal
    {
        private readonly IFileSystem _fileSystem;
        private ConcurrentDictionary<string, string> _settings = new ConcurrentDictionary<string, string>();
        private readonly object _lock = new object();

        private const string SETTINGS_FILENAME = "settings.xml";
        private readonly string _settingsPath;
        private DateTime _nextForcedReadTime = DateTime.MinValue;
        private DateTime _lastFileWriteTime = DateTime.MinValue;
        private readonly TimeSpan _forceReadInterval = TimeSpan.FromMinutes(1);
        private readonly IDateTimeProvider _dateTimeProvider;
        private string _currentSettingsString = string.Empty;

        public SettingsInternal(IFileSystem fileSystem, IDateTimeProvider dateTimeProvider)
        {
            _fileSystem = fileSystem;
            _dateTimeProvider = dateTimeProvider;
            _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SETTINGS_FILENAME);
        }

        public string[] FindSettings(string startsWith, StringComparison stringComparison)
        {
            lock (_lock)
            {
                ReloadIfNeeded();
                return (from kvp in _settings
                        where kvp.Key.StartsWith(startsWith, stringComparison)
                        select kvp.Key).ToArray();
            }
        }

        public void RemoveSetting(string key)
        {
            lock (_lock)
            {
                ReloadIfNeeded();

                string value;
                _settings.TryRemove(key, out value);

                WriteXml();
            }
        }

        public int GetInt32(string key, int defaultValue)
        {
            var valueAsString = GetSetting(key);
            if (string.IsNullOrEmpty(valueAsString))
            {
                return defaultValue;
            }

            return int.Parse(valueAsString);
        }

        public long GetInt64(string key, long defaultValue)
        {
            var valueAsString = GetSetting(key);
            if (string.IsNullOrEmpty(valueAsString))
            {
                return defaultValue;
            }

            return long.Parse(valueAsString);
        }

        public bool GetBool(string key, bool defaultValue)
        {
            var valueAsString = GetSetting(key);
            if (string.IsNullOrEmpty(valueAsString))
            {
                return defaultValue;
            }

            return bool.Parse(valueAsString);
        }

        public Guid GetGuid(string key, Guid defaultValue)
        {
            var valueAsString = GetSetting(key);
            if (string.IsNullOrEmpty(valueAsString))
            {
                return defaultValue;
            }

            return Guid.Parse(valueAsString);
        }

        public Uri GetUri(string key, Uri defaultValue)
        {
            var valueAsString = GetSetting(key);
            if (string.IsNullOrEmpty(valueAsString))
            {
                return defaultValue;
            }

            return new Uri(valueAsString);
        }

        public TimeSpan GetTimeSpan(string key, TimeSpanSerializedUnit unit, TimeSpan defaultValue)
        {
            string valueAsString = GetSetting(key)
                .Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator)
                .Replace(".", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);

            if (string.IsNullOrEmpty(valueAsString))
            {
                return defaultValue;
            }

            double numericalValue = double.Parse(valueAsString, NumberStyles.Number, CultureInfo.InvariantCulture);

            switch (unit)
            {
                case TimeSpanSerializedUnit.Milliseconds:
                    return TimeSpan.FromMilliseconds(numericalValue);
                case TimeSpanSerializedUnit.Seconds:
                    return TimeSpan.FromSeconds(numericalValue);
                case TimeSpanSerializedUnit.Minutes:
                    return TimeSpan.FromMinutes(numericalValue);
                case TimeSpanSerializedUnit.Hours:
                    return TimeSpan.FromHours(numericalValue);
                case TimeSpanSerializedUnit.Days:
                    return TimeSpan.FromDays(numericalValue);
                default:
                    throw new ArgumentOutOfRangeException(nameof(unit), unit, null);
            }
        }

        public IEnumerable<string> GetStringList(string key, char itemSeparator)
        {
            var valueAsString = GetSetting(key);
            if (string.IsNullOrEmpty(valueAsString))
            {
                return new string[] { };
            }

            return valueAsString.Split(itemSeparator);
        }

        public IEnumerable<Guid> GetGuidList(string key, char itemSeparator)
        {
            var valueAsString = GetSetting(key);
            if (string.IsNullOrEmpty(valueAsString))
            {
                return new Guid[] { };
            }

            return valueAsString.Split(itemSeparator)
                .Select(Guid.Parse)
                .ToArray();
        }

        public IEnumerable<Uri> GetUriList(string key, char itemSeparator)
        {
            var valueAsString = GetSetting(key);
            if (string.IsNullOrEmpty(valueAsString))
            {
                return new Uri[] { };
            }

            return valueAsString
                .Split(itemSeparator)
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => new Uri(x))
                .ToArray();
        }

        public string GetSetting(string key)
        {
            return GetSetting(key, string.Empty);
        }

        public string GetSetting(string key, string defaultValue)
        {
            lock (_lock)
            {
                ReloadIfNeeded();
                string value = _settings.TryGetValue(key, out value) ? value : defaultValue;
                return value;
            }
        }

        public void SetSetting(string key, string value)
        {
            lock (_lock)
            {
                ReloadSettings();
                _settings.AddOrUpdate(key, x => value, (x, oldValue) => value);
                WriteXml();
            }
        }

        public String[] AllKeys()
        {
            lock (_lock)
            {
                return _settings.Keys.ToArray();
            }
        }


        private void ReloadIfNeeded()
        {
            if (!_settings.Any() ||
                (_dateTimeProvider.Time() > _nextForcedReadTime) ||
                (_lastFileWriteTime != _fileSystem.GetLastWriteTimeUtc(_settingsPath)))
            {
                ReloadSettings();
            }
        }

        private void ReloadSettings()
        {
            var newSettings = ReadXml(_settingsPath, false);
            var updatedKeys = GetUpdatedKeys(newSettings);
            _settings = newSettings;

            if (updatedKeys.Any())
            {
                OnSettingsUpdated(updatedKeys);
            }
        }

        private string[] GetUpdatedKeys(ConcurrentDictionary<string, string> newSettings)
        {
            var removedKeys = _settings.Keys.Except(newSettings.Keys).ToArray();
            var addedKeys = newSettings.Keys.Except(_settings.Keys).ToArray();
            var changedKeys = _settings.Where(originalPair =>
                {
                    string newValue;
                    return !removedKeys.Contains(originalPair.Key)
                           && newSettings.TryGetValue(originalPair.Key, out newValue)
                           && newValue != originalPair.Value;
                })
                .Select(originalPair => originalPair.Key)
                .ToArray();
            var updatedKeys = removedKeys.Union(addedKeys).Union(changedKeys);
            return updatedKeys.ToArray();
        }

        private void OnSettingsUpdated(IEnumerable<string> updatedKeys)
        {
            Log.To.Main.Add($"The following setting keys have been updated: {string.Join(", ", updatedKeys)}", LogLevel.Info);
        }

        private ConcurrentDictionary<string, string> ReadXml(string path, bool justTesting)
        {
            var result = new ConcurrentDictionary<string, string>();

            try
            {
                if (_fileSystem.FileExists(path))
                {
                    IEnumerable<XElement> elements;
                    using (TextReader reader = _fileSystem.GetReader(path))
                    {
                        elements = XDocument.Load(reader).Descendants("node");
                    }
                    result = new ConcurrentDictionary<string, string>(elements.ToDictionary(element => element.Attribute("key").Value, element => element.Attribute("value").Value));

                    var newSettingsString = string.Join(", ", result.Select(pair => $"{pair.Key}=\"{pair.Value}\""));
                    if (newSettingsString != _currentSettingsString)
                    {
                        _currentSettingsString = newSettingsString;

                        if (!justTesting)
                        {
                            Log.To.Main.Add($"Renewed settings: {_currentSettingsString}", LogLevel.Info);
                        }
                    }

                    if (!justTesting)
                    {
                        _nextForcedReadTime = _dateTimeProvider.Time().Add(_forceReadInterval);
                        _lastFileWriteTime = _fileSystem.GetLastWriteTimeUtc(path);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.To.Main.AddException("Error when reading settings", ex);
            }

            return result;
        }

        private void WriteXml()
        {
            int i = 0;
            var destinationFileName = _settingsPath;
            string filename = destinationFileName + "_" + Guid.NewGuid();
            bool writeWasSucessful;
            do
            {
                WriteXmlImpl(filename);
            }
            while (!(writeWasSucessful = ValidateXmlFile(filename)) && ++i < 10);

            if (!writeWasSucessful)
            {
                Log.To.Main.Add("Critical error writing XML settings file", LogLevel.Verbose);
                return;
            }

            if (_fileSystem.FileExists(destinationFileName))
            {
                _fileSystem.ReplaceFile(filename, destinationFileName, filename + "_fsCopy");
                if (_fileSystem.FileExists(filename + "_fsCopy"))
                    _fileSystem.DeleteFile(filename + "_fsCopy");
            }
            else
            {
                _fileSystem.MoveFile(filename, destinationFileName);
            }
        }

        private void WriteXmlImpl(string filename)
        {
            try
            {
                XDocument doc = new XDocument();
                doc.Add(new XElement("root", GetSettingsElements()));
                using (TextWriter writer = _fileSystem.GetWriter(filename))
                {
                    doc.Save(writer);
                }
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        private IEnumerable<XElement> GetSettingsElements()
        {
            return _settings
                .OrderBy(t => t.Key)
                .Select(setting => new XElement("node", new XAttribute("key", setting.Key), new XAttribute("value", setting.Value)));
        }


        private bool ValidateXmlFile(string filename)
        {
            try
            {
                if (!_fileSystem.FileExists(filename)) return false;

                var settings = ReadXml(filename, true);

                if (settings.Count != _settings.Count) return false;
                foreach (var kvp in settings)
                {
                    if (!_settings.ContainsKey(kvp.Key)) return false;
                    if (!_settings[kvp.Key].Equals(kvp.Value)) return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetAbsoluteLocalPath(string key, string baseDirectory, string defaultValue)
        {
            var pathValue = GetSetting(key, defaultValue);
            baseDirectory = string.IsNullOrEmpty(baseDirectory) ? Environment.CurrentDirectory : baseDirectory;
            return new Uri(Path.Combine(baseDirectory, pathValue)).LocalPath;
        }
    }
}