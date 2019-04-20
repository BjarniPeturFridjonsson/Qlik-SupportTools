using System;
using System.Collections.Generic;
using System.Diagnostics;
using Eir.Common.IO;
using Eir.Common.Logging;

namespace Gjallarhorn.SenseLogReading
{
    public class LogFileFinder
    {
        private readonly Action<IFileInfo, string> _fileWriter;

        private readonly IFileSystem _fileSystem;
        private readonly Func<string, bool> _ignorePaths;

        public LogFileFinder(Action<IFileInfo, string> fileWriter, IFileSystem fileSystem,  Func<string, bool> ignorePaths)
        {
            _fileSystem = fileSystem;
            _fileWriter = fileWriter;
            _ignorePaths = ignorePaths;
        }

        public void FindFilesInDirectories(DirectorySetting rootDir, DateTime from, DateTime to, string outputFilePath)
        {
            GetFileDirectories(rootDir, from, to, outputFilePath);
        }

        private void GetFileDirectories(DirectorySetting rootDir, DateTime from, DateTime to, string outputFilePath)
        {
            if (_ignorePaths != null)
            {
                if (!_ignorePaths.Invoke(rootDir.Name))
                {
                    Trace.WriteLine($"Igoring=>{rootDir.Name}");
                    return;
                }
            }
            GetFiles(rootDir.Path, from, to, outputFilePath);
            _fileWriter(null, null);//dir counter hack.
            foreach (string dir in rootDir.GetDirectories())
            {
                GetFileDirectories(new DirectorySetting(dir), from, to, _fileSystem.Path.Combine(outputFilePath, _fileSystem.Path.GetFileName(dir) + ""));
            }
        }

        private void GetFiles(string dir, DateTime from, DateTime to, string outputFilePath)
        {
            try
            {
                if (!_fileSystem.DirectoryExists(dir))
                {
                    Log.To.Main.Add($"The directory {dir} does not exist");
                    return;
                }

                DirectoryInfo info = new DirectoryInfo();
                Trace.WriteLine($"start => {dir}");
                var files = info.EnumerateLogFiles(dir, from, to);
                foreach (IFileInfo file in files)
                {
                    file.Refresh(); //some files are returning empty even though they are not. Shown 0 bytes in explorer until opened in notepad. 
                    _fileWriter(file, outputFilePath);
                }

                var configs = info.EnumerateFilesByExtension(dir, new List<string> { ".ini", ".config", ".xml", ".pgo" });
                foreach (IFileInfo file in configs)
                {
                    _fileWriter(file, outputFilePath);
                }

                Trace.WriteLine($"stop  => {dir}");
            }
            catch (Exception e)
            {
                Log.To.Main.AddException($"Failed getting files on path {dir}", e);
            }
        }
    }
}
