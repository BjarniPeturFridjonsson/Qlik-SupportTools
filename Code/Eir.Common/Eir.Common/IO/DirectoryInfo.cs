using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Eir.Common.IO
{
    public class DirectoryInfo :IDirectoryInfo
    {
        public IEnumerable<IFileInfo> GetFiles(string path)
        {
            
            var a =  new System.IO.DirectoryInfo(path).GetFiles();
            var ret = new List<IFileInfo>();
            foreach (var fileInfo in a)
            {
                ret.Add(new SystemFileInfo(fileInfo));
            }
            return ret;
        }

        public IEnumerable<IFileInfo> EnumerateFilesByExtension(string path, List<string> extensions)
        {
            var sw = new Stopwatch();
            sw.Start();
            
            var dirInfo = new System.IO.DirectoryInfo(path);

            var a = dirInfo.EnumerateFiles().Where(p =>
                extensions.Contains(p.Extension,StringComparer.InvariantCultureIgnoreCase)
            );
            var ret = new List<IFileInfo>();
            foreach (var fileInfo in a)
            {
                ret.Add(new SystemFileInfo(fileInfo));
            }
            sw.Stop();
            Debug.WriteLine($"enum time => {sw.Elapsed.TotalSeconds} for {path}");
            return ret;
        }

        public IEnumerable<IFileInfo> EnumerateLogFiles(string path, DateTime from, DateTime to)
        {
            var sw = new Stopwatch();
            sw.Start();
            var dirInfo = new System.IO.DirectoryInfo(path);

            var a = dirInfo.EnumerateFiles().Where(p=>
                p.LastWriteTime >= from && p.LastWriteTime <= to && 
                (p.Extension.Equals(".log", StringComparison.InvariantCultureIgnoreCase) || p.Extension.Equals(".txt", StringComparison.InvariantCultureIgnoreCase))
            );

            var ret = new List<IFileInfo>();
            foreach (var fileInfo in a)
            {
                ret.Add(new SystemFileInfo(fileInfo));
            }
            sw.Stop();
            Debug.WriteLine($"enum time => {sw.Elapsed.TotalSeconds} for {path}");
            return ret;
        }
    }
}
