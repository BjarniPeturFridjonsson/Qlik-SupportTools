using System;
using System.Collections.Generic;
using Eir.Common.IO;
using FreyrCommon.Models;
using Newtonsoft.Json;

namespace FreyrViewer.Common
{
    public class SimpleFileDataParser
    {
        private readonly IFileSystem _fileSystem;

        public SimpleFileDataParser(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public List<SuperSimpleColumnTypes.TwoColumnType> ParseJson2Column(string filepath)
        {
            string data = _fileSystem.GetReader(filepath)?.ReadToEnd();
            if (data == null || string.IsNullOrEmpty(data) || data.Equals("null"))
                return null;
            return SuperSimpleColumnTypes.SerializeJsonInto<SuperSimpleColumnTypes.TwoColumnType>(data);
        }

        public List<T> ParseJsonFileDataList<T>(string filePath, Action<string> serializationErrors)
        {

            //dfa
            var sr = _fileSystem.GetReader(filePath);
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Error += (sender, args) =>
                {
                    serializationErrors?.Invoke(args.ErrorContext.Error.Message);
                    args.ErrorContext.Handled = true;
                };

                // read the json from a stream
                var p = serializer.Deserialize<List<T>>(reader);
                return p;
            }

        }
        public T ParseJsonFileData<T>(string filePath)
        {
            string data = _fileSystem.GetReader(filePath)?.ReadToEnd();
            var ret = JsonConvert.DeserializeObject<T>(data);
            return ret;
        }
    }
}
