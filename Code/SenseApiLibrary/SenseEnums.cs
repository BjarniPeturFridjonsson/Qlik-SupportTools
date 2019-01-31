using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;

namespace SenseApiLibrary
{
    public class SenseEnums
    {
        private static readonly char[] _enumKeyValueSeparator = { ':' };

        private readonly Lazy<JToken> _lazyEnumJson;
        private readonly ConcurrentDictionary<string, Dictionary<int, string>> _dictDict;

        public SenseEnums(SenseApiSupport senseApiSupport)
        {
            _dictDict = new ConcurrentDictionary<string, Dictionary<int, string>>();
            _lazyEnumJson = new Lazy<JToken>(() => GetEnumJson(senseApiSupport));
        }

        public bool TryGetValue(string enumName, int key, out string value)
        {
            Dictionary<int, string> dict = _dictDict.GetOrAdd(enumName, GetDict);
            return dict.TryGetValue(key, out value);
        }

        public string GetValue(string enumName, int key, string defaultValue)
        {
            string value;
            return TryGetValue(enumName, key, out value) ? value : defaultValue;
        }

        private static dynamic GetEnumJson(SenseApiSupport senseApiSupport)
        {
            try
            {
                return senseApiSupport.RequestWithResponse(
                    ApiMethod.Get,
                    $"https://{senseApiSupport.Host}:4242/qrs/about/api/enums",
                    null,
                    null,
                    HttpStatusCode.OK,
                    JToken.Parse);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return null;
            }
        }

        private Dictionary<int, string> GetDict(string enumName)
        {
            try
            {
                dynamic jsonStruct = _lazyEnumJson.Value[enumName];

                //yeah thx for that R&D
                JArray array = (JArray)jsonStruct.values ?? (JArray)jsonStruct.Values;
                
                return array.Select(x => x.ToString()
                    .Split(_enumKeyValueSeparator, 2))
                    .Where(x => x.Length == 2)
                    .ToDictionary(x => int.Parse(x[0].Trim()), x => x[1].Trim());
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
                return new Dictionary<int, string>();
            }
        }
    }
}