using System;
using System.Collections.Generic;
using Eir.Common.IO;

namespace Eir.Common.Common
{
    public static class Settings
    {
        private static SettingsInternal _instance;

        static Settings()
        {
            _instance = new SettingsInternal(FileSystem.Singleton, DateTimeProvider.Singleton);
        }

        /// <summary>
        /// Sets the instance used for reading and writing settings to <paramref name="settingsInternal"/>, and returns 
        /// the previous instance.
        /// THIS IS FOR TESTING PURPOSES ONLY.
        /// </summary>
        public static SettingsInternal ExchangeInstance(SettingsInternal settingsInternal)
        {
            var previousSettingsInternal = _instance;
            _instance = settingsInternal;
            return previousSettingsInternal;
        }

        public static string[] FindSettings(string startsWith, StringComparison stringComparison)
        {
            return _instance.FindSettings(startsWith, stringComparison);
        }

        public static void RemoveSetting(string key)
        {
            _instance.RemoveSetting(key);
        }

        public static int GetInt32(string key, int defaultValue) => _instance.GetInt32(key, defaultValue);

        public static long GetInt64(string key, long defaultValue) => _instance.GetInt64(key, defaultValue);

        public static bool GetBool(string key, bool defaultValue) => _instance.GetBool(key, defaultValue);

        public static Guid GetGuid(string key, Guid defaultValue) => _instance.GetGuid(key, defaultValue);

        public static Uri GetUri(string key, Uri defaultValue) => _instance.GetUri(key, defaultValue);

        public static TimeSpan GetTimeSpan(string key, TimeSpanSerializedUnit unit, TimeSpan defaultValue) => _instance.GetTimeSpan(key, unit, defaultValue);


        public static IEnumerable<string> GetStringList(string key, char itemSeparator) => _instance.GetStringList(key, itemSeparator);

        public static IEnumerable<Guid> GetGuidList(string key, char itemSeparator) => _instance.GetGuidList(key, itemSeparator);

        public static IEnumerable<Uri> GetUriList(string key, char itemSeparator) => _instance.GetUriList(key, itemSeparator);

        public static string GetAbsoluteLocalPath(string key, string baseDirectory = null, string defaultValue = null) => _instance.GetAbsoluteLocalPath(key, baseDirectory, defaultValue);

        public static string GetSetting(string key)
        {
            return _instance.GetSetting(key);
        }

        public static string GetSetting(string key, string defaultValue)
        {
            return _instance.GetSetting(key, defaultValue);
        }

        public static void SetSetting(string key, string value)
        {
            _instance.SetSetting(key, value);
        }

        public static string[] AllKeys()
        {
            return _instance.AllKeys();
        }
    }
}