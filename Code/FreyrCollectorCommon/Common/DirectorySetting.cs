//using System;
//using System.Collections.Generic;
//using System.IO;

//namespace FreyrCollectorCommon.Common
//{
//    public class DirectorySetting
//    {
//        public static readonly DirectorySetting Empty = new DirectorySetting(string.Empty);

//        public static readonly IEqualityComparer<DirectorySetting> Comparer = new DirectorySettingComparer();

//        private class DirectorySettingComparer : IEqualityComparer<DirectorySetting>
//        {
//            public bool Equals(DirectorySetting x, DirectorySetting y)
//            {
//                return string.Equals(x.Path, y.Path, StringComparison.OrdinalIgnoreCase);
//            }

//            public int GetHashCode(DirectorySetting obj)
//            {
//                return obj.Path.ToLower().GetHashCode();
//            }
//        }

//        public DirectorySetting(string path)
//        {
//            Path = path?.Trim() ?? string.Empty;
//        }

//        public string Path { get; }

//        public string Name => System.IO.Path.GetFileName(Path);

//        public DirectorySetting ParentDirectory => new DirectorySetting(System.IO.Path.GetDirectoryName(Path));

//        public bool Exists => !string.IsNullOrEmpty(Path) && Directory.Exists(Path);

//        public DirectoryInfo GetDirectoryInfo()
//        {
//            return new DirectoryInfo(Path);
//        }

//        public bool ChildExists(string childPathPart)
//        {
//            return Directory.Exists(System.IO.Path.Combine(Path, childPathPart));
//        }

//        public string[] GetDirectories()
//        {
//            return Directory.GetDirectories(Path);
//        }

//        public override string ToString() => Path;
//    }
//}