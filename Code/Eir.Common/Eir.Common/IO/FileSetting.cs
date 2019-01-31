using System;
using System.Collections.Generic;
using System.IO;

namespace Eir.Common.IO
{
    public class FileSetting
    {
        public static readonly FileSetting Empty = new FileSetting(string.Empty);

        public static readonly IEqualityComparer<FileSetting> Comparer = new FilePathSettingComparer();

        private class FilePathSettingComparer : IEqualityComparer<FileSetting>
        {
            public bool Equals(FileSetting x, FileSetting y)
            {
                return string.Equals(x.Path, y.Path, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(FileSetting obj)
            {
                return obj.Path.ToLower().GetHashCode();
            }
        }

        public FileSetting(string path)
        {
            Path = path?.Trim() ?? string.Empty;
        }

        public string Path { get; }

        public string Name => System.IO.Path.GetFileName(Path);

        public DirectorySetting ParentDirectory => new DirectorySetting(System.IO.Path.GetDirectoryName(Path));

        public bool Exists => !string.IsNullOrEmpty(Path) && File.Exists(Path);

        public FileInfo GetFileInfo()
        {
            return new FileInfo(Path);
        }

        public override string ToString() => Path;
    }
}