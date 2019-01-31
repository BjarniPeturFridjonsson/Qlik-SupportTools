using System;
using System.IO;
using System.IO.Compression;

namespace FreyrCollectorCommon.CollectorCore
{
    public class Zipper
    {
        private readonly string _folderPath;

        public Zipper(string folderPath)
        {
            _folderPath = folderPath;
        }

        public string ZipFolder(string folderPath, string fileNameStart,string pathToOutputZip)
        {
            
            var outputPath = Path.Combine(pathToOutputZip, fileNameStart + Path.GetRandomFileName() +  ".zip");
            ZipFile.CreateFromDirectory(folderPath, outputPath, CompressionLevel.Fastest, false);
            return outputPath;
        }

        public string ZipOneFile(FileInfo file)
        {
            var tmpFile = Path.Combine(_folderPath, Guid.NewGuid() + ".zip");
            using (FileStream fs = new FileStream(tmpFile, FileMode.Create))
            using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                CreateEntry(arch,file);
            }
            return tmpFile;
        }

        public string ZipFiles(FileInfo[] file)
        {
            var tmpFile = Path.Combine(_folderPath, Guid.NewGuid() + ".zip");
            using (FileStream fs = new FileStream(tmpFile, FileMode.Create))
            using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                foreach (var item in file)
                {
                    CreateEntry(arch, item);
                }

            }
            return tmpFile;
        }

        private void CreateEntry(ZipArchive destination, FileInfo file)
        {
            using (Stream stream = File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                ZipArchiveEntry zipArchiveEntry =  destination.CreateEntry(file.Name, CompressionLevel.Fastest);
                using (Stream destination1 = zipArchiveEntry.Open())
                {
                    stream.CopyTo(destination1);
                }
                //return zipArchiveEntry;
            }
        }
    }
}
