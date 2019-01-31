using System;
using System.IO;

namespace Eir.Common.IO
{
    public class SystemFileInfo : IFileInfo
    {
        private FileInfo _fi;

        public SystemFileInfo(FileInfo fi)
        {
            Populate(fi);
        }

        public SystemFileInfo(string path)
        {
            Populate(new FileInfo(path));
        }

        private void  Populate(FileInfo fi)
        {
            _fi = fi;
            FullName = fi.FullName;
            Name = fi.Name;
            Length = fi.Length;
            Extension = fi.Extension;
        }

        public string FullName { get; private set; }
        public string Name { get; private set; }
        public string Extension { get; private set; }
        public long Length { get; private set; }
        public DateTime LastWriteTimeUtc => _fi.LastWriteTimeUtc;
        public DateTime CreationTime  => _fi.CreationTime;
        public DateTime LastWriteTime => _fi.LastWriteTime;
        public void Refresh() => _fi.Refresh();
    }
}
