using System;
using System.IO;
using System.Text;
using FreyrCommon.Logging;

namespace FreyrQvLogCollector.QvCollector
{
    internal class IniFileSupport
    {
        private readonly ILogger _logger;

        public IniFileSupport(ILogger logger)
        {
            _logger = logger;
        }

        public bool TryFindValue(string path, string section, string key, out string value)
        {
            try
            {
                var isInSection = false;

                using (Stream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine()?.Trim() ?? string.Empty;

                            if (line.StartsWith("["))
                            {
                                isInSection = line.StartsWith("[" + section + "]");
                            }
                            else if (isInSection && line.StartsWith(key))
                            {
                                var parts = line.Split('=');
                                if (parts.Length == 2)
                                {
                                    value = parts[1];
                                    return true;
                                }
                            }
                        }
                    }
                }

                value = null;
                return false;
            }
            catch (Exception ex)
            {
                _logger.Add("Inifile failed getting TryFindValue", ex);
                value = null;
                return false;
            }
        }
    }
}
