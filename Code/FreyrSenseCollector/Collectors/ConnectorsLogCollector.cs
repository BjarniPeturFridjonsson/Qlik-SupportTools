using System.Diagnostics;
using System.Threading.Tasks;
using Eir.Common.IO;
using FreyrCommon.Models;
using FreyrCollectorCommon.LogFileReader;
using FreyrCommon.Logging;


namespace FreyrSenseCollector.Collectors
{
    public class ConnectorsLogCollector
    {
        public int FoundFileCount { get; set; }
        private readonly IFileSystem _fileSystem;
        private CommonCollectorServiceVariables _settings;
        private readonly ILogger _logger;

        public ConnectorsLogCollector(IFileSystem fileSystem,ILogger logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
        }

        public async Task<bool> GetLogs(string path, CommonCollectorServiceVariables settings, string outputFilePath)
        {
            try
            {
                _settings = settings;
                FindLogFiles(_fileSystem.Path.Combine(path, "ProgramData\\Qlik\\Custom Data"), _fileSystem.Path.Combine(outputFilePath, "ConnectorLogs"));
            }
            catch
            {
                _logger.Add($"Failed getting Connector GetLogs on {path}");
                return false;
            }

            return true;
        }


        private void FindLogFiles(string dir, string outputFilePath)
        {
            var finder = new LogFileFinder(WriteFile, _fileSystem, _logger,null);
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
