using System;
using System.Collections.Generic;
using System.IO;

namespace Eir.Common.IO
{
    public class FileSystem : IFileSystem
    {
        public static IFileSystem Singleton { get; } = new FileSystem();
        public IFilePath Path => new FilePath();

        private FileSystem()
        {
        }

        public bool FileExists(string path)
        {
            if (!path.StartsWith(@"\\"))
            {
                return File.Exists(path);
            }

            var fi = new FileInfo(path);
            if (!fi.Exists)
            {
                fi.Refresh();//whoahhahahahah
            }

            return fi.Exists;
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public void DeleteDirectory(string path, bool recursive)
        {
            Directory.Delete(path, recursive);
        }

        public void MoveFile(string sourcePath, string destPath)
        {
            File.Move(sourcePath, destPath);
        }

        public void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void ReplaceFile(string sourcePath, string destPath, string destBackupPath)
        {
            File.Replace(sourcePath, destPath, destBackupPath);
        }

        public IEnumerable<string> EnumerateFiles(string path, string pattern)
        {
            return Directory.EnumerateFiles(path, pattern);
        }

        public IEnumerable<string> EnumerateDirectories(string source)
        {
            return Directory.EnumerateDirectories(source);
        }

        public Stream GetStream(string path, FileMode fileMode, FileAccess fileAccess)
        {
            return new FileStream(path, fileMode, fileAccess, FileShare.Read, 4096, FileOptions.Asynchronous);
        }

        public long GetFileSize(string filename)
        {
            return new FileInfo(filename).Length;
        }

        public DateTime GetLastWriteTimeUtc(string filename)
        {
            return new FileInfo(filename).LastWriteTimeUtc;
        }

        public IFileInfo GetFileInfo(string path)
        {
            return new SystemFileInfo(path);
        }

        public void FileCopy(string sourceFile, string destinationFile)
        {
            File.Copy(sourceFile,destinationFile);
        }

        public IEnumerable<string> GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public IEnumerable<string> DirectoryGetFiles(string path)
        {
            return Directory.GetFiles(path);
        }
    }
}