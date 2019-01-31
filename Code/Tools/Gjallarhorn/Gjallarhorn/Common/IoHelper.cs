using System;
using System.IO;

namespace Gjallarhorn.Common
{
    public static class IoHelper
    {
        /// Returns true if the path is a dir, false if it's a file and null if it's neither or doesn't exist. 
        public static bool? IsPathDirOrFile(this string path)
        {
            bool? result = null;
            if (Directory.Exists(path) || File.Exists(path))
            {
                // get the file attributes for file or directory 
                var fileAttr = File.GetAttributes(path);
                if (fileAttr.HasFlag(FileAttributes.Directory))
                    result = true;
                else
                    result = false;
            }
            return result;
        }
    }
}
