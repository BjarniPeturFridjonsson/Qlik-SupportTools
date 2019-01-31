using System;
using System.Diagnostics;
using System.IO;

namespace FreyrCollectorCommon.CollectorCore
{
    public class GetOutputFolder

    {
        public string FallbackRutine(string baseName)

        {
            try
            {
                var appPath = AppDomain.CurrentDomain.BaseDirectory;
                if(appPath.Length < 4)
                    throw new Exception("Trying to run from root folder");
                var path = Path.Combine(appPath, $"{baseName}_{Guid.NewGuid()}");
                if (Directory.Exists(path))
                    throw new Exception("Folder exits, inconcivable!");
                Directory.CreateDirectory(path);
                File.WriteAllText(Path.Combine(path, "location.txt"), @"Running in app domain path");
                return path;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }

            try
            {
                var path = Path.Combine("C:\\Users\\Public\\Documents", $"{baseName}_{Guid.NewGuid()}");
                if (Directory.Exists(path))
                    throw new Exception("Folder exits, really inconcivable!");
                Directory.CreateDirectory(path);
                File.WriteAllText(Path.Combine(path, "location.txt"), @"Running in public documents path");
                return path;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }

           
            var lastDitchPath = Path.Combine(Path.GetTempPath(), $"{baseName}_{Guid.NewGuid()}");
            if (Directory.Exists(lastDitchPath))
                throw new Exception("Folder exits, thats inconcivable!");
            Directory.CreateDirectory(lastDitchPath);
            File.WriteAllText(Path.Combine(lastDitchPath, "location.txt"), @"Running in user temp path");
            return lastDitchPath;
           
        }
    }
}
