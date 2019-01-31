using System;
using System.IO;
using System.Text;
using System.Threading;
using FreyrCollectorCommon.CollectorCore;
using FreyrCommon.Logging;
using FreyrCommon.Models;
using Newtonsoft.Json;

namespace FreyrCollectorCommon.Common
{
    public class CollectorHelper
    {
        private readonly CommonCollectorServiceVariables _settings;
        private readonly ILogger _logger;

        public CollectorHelper(CommonCollectorServiceVariables settings, ILogger logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public void RunAction(Func<object> func, string name)
        {
            try
            {
                _logger.Add($"Starting running {name}");
                object res = func();
                WriteContentToFile(res, name);
            }
            catch (Exception e)
            {
                Log.Add($"Failed running {name}", e);
            }
        }

        public string CreateUniqueFileName(string name)
        {
            var tempPath = Path.Combine(_settings.OutputFolderPath, name + "_" + Guid.NewGuid() + ".json");
            Directory.CreateDirectory(Path.GetDirectoryName(tempPath) + "");
            return tempPath;
        }

        public void WriteContentToFile(object content, string name)
        {
            var tempPath = CreateUniqueFileName(name);
            //File.WriteAllText(tempPath, JsonConvert.SerializeObject(content, Formatting.Indented));
            byte[] data;
            if (content is string s)
            {
                data = Encoding.UTF8.GetBytes(s);
            }
            else
            {
                data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(content, Formatting.Indented));
            }

            using (var tempFile = File.Create(tempPath, 4096, FileOptions.WriteThrough))
                tempFile.Write(data, 0, data.Length);
            //the files may not yet be on disk yet
            _logger.Add(WaitForFileFlushedToDisk(new FileInfo(tempPath), data.Length)
                ? $"Writing file {tempPath} to disk finished"
                : $"Waiting time for file to be written has been exceeded. The file {tempPath} might be missing");
        }

        private bool WaitForFileFlushedToDisk(FileInfo file, long fileSize, int msecTimeout=5000)
        {
            if (fileSize < 1) return true;

            FileStream stream = null;
            int runningTime = 0;

            while (true)
            {
                try
                {
                    stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                    if (stream.Length > 0)
                        return true;
                }
                catch (IOException)
                {
                    //the file is unavailable because it is:
                    //still being written to
                    //or being processed by another thread
                    //or does not exist
                    
                }
                finally
                {
                    stream?.Close();
                }
                Thread.Sleep(200);
                runningTime += 200;
                if (runningTime > msecTimeout)
                {
                    return false;
                }
            }
            
        }

        public string CreateZipFile(CommonCollectorServiceVariables serviceVariables)
        {
            string path = serviceVariables.OutputFolderPath;
            var a = new Zipper(path);
            var fileName = serviceVariables.ApplicatonBaseName + "_" + serviceVariables.Key;
            return a.ZipFolder(path, fileName, Path.GetFullPath(Path.Combine(serviceVariables.OutputFolderPath, @"..")));
        }
    }
}
