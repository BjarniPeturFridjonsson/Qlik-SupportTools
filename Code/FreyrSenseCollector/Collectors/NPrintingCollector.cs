using System.Diagnostics;
using System.Threading.Tasks;
using Eir.Common.IO;
using FreyrCommon.Models;
using FreyrCollectorCommon.LogFileReader;
using FreyrCommon.Logging;


namespace FreyrSenseCollector.Collectors
{
    public class NPrintingCollector
    {
        public int FoundFileCount { get; set; }

        private readonly IFileSystem _fileSystem;
        private CommonCollectorServiceVariables _settings;
        private readonly ILogger _logger;

        public NPrintingCollector(IFileSystem fileSystem, ILogger logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
        }

        public async Task<bool> GetLogs(string path, CommonCollectorServiceVariables settings, string outputFilePath)
        {
            try
            {
                _settings = settings;
                await WriteNPrintingInfo(path, outputFilePath).ConfigureAwait(false);
                FindLogFiles(_fileSystem.Path.Combine(path, "ProgramData\\NPrinting\\Logs"), _fileSystem.Path.Combine(outputFilePath, "NPrintingLogs"));
            }
            catch
            {
                _logger.Add($"Failed getting NPrinting GetLogs on {path}");
                return false;
            }
           
            return true;
        }

        private async Task<bool> WriteNPrintingInfo(string path, string outputFolderPath)
        {
            try
            {
                string engineInfo = await _fileSystem.ReadFileContentAsync(_fileSystem.Path.Combine(path, "Program Files\\NPrintingServer\\engine-build-info.txt")).ConfigureAwait(false);
                string serverInfo = await _fileSystem.ReadFileContentAsync(_fileSystem.Path.Combine(path, "Program Files\\NPrintingServer\\server-build-info.txt")).ConfigureAwait(false);
                _fileSystem.EnsureDirectory(outputFolderPath);
                using (var writer = _fileSystem.GetWriter(_fileSystem.Path.Combine(outputFolderPath, "NPrintingInfo.txt")))
                {
                    writer.WriteLine("engine-build-info=>" + engineInfo);
                    writer.WriteLine("server-build-info=>" + serverInfo);
                    writer.Close();
                }
                    
            }
            catch
            {
                Log.Add($"Failed getting NPrinting info on {path}");
                return false;
            }
            return true;
        }

        private void FindLogFiles(string dir,string outputFilePath)
        {
            var finder = new LogFileFinder(WriteFile, _fileSystem, _logger, null);
            finder.FindFilesInDirectories(new DirectorySetting(dir), _settings.StartDateForLogs, _settings.StopDateForLogs, outputFilePath);
        }

        private void WriteFile(IFileInfo file, string outputFilePath)
        {
            Trace.WriteLine($"{outputFilePath} => {file.Name}");
            _fileSystem.EnsureDirectory(outputFilePath);
            string outputFileFullPath = _fileSystem.Path.Combine(outputFilePath, file.Name);
            if (_fileSystem.FileExists(outputFileFullPath))
                outputFileFullPath += "_" + _fileSystem.Path.GetFileNameWithoutExtension(_fileSystem.Path.GetTempFileName());
            _fileSystem.FileCopy(file.FullName, outputFileFullPath);
            FoundFileCount++;
        }
    }
}
