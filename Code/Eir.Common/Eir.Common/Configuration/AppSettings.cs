using System;
using System.Collections.Specialized;
using System.Configuration;
using Eir.Common.Common;

namespace Eir.Common.Configuration
{
    public interface IAppSettings
    {
        string Get(string key);
        string Get(string key, string defaultValue);
    }

    public class AppSettings : IAppSettings
    {
        private readonly IDateTimeProvider _dateTimeProvider = DateTimeProvider.Singleton;
        private DateTime _latestConfigRefreshTime = DateTime.MinValue;
        private readonly TimeSpan _configRefreshThreshold = TimeSpan.FromSeconds(15);


        public string Get(string key)
        {
            return Get(key, string.Empty);
        }

        public string Get(string key, string defaultValue)
        {
            return GetAll()[key] ?? defaultValue;
        }

        private void RefreshIfNeeded()
        {
            var now = _dateTimeProvider.Time();
            if (now.Subtract(_latestConfigRefreshTime) > _configRefreshThreshold)
            {
                ConfigurationManager.RefreshSection("appSettings");
                _latestConfigRefreshTime = now;
            }
        }

        public NameValueCollection GetAll()
        {
            RefreshIfNeeded();
            return ConfigurationManager.AppSettings;
        }
    }
}