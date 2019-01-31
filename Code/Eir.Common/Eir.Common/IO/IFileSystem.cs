using System;
using System.Collections.Generic;
using System.IO;

namespace Eir.Common.IO
{
    public interface IFileSystem
    {
        bool FileExists(string path);

        bool DirectoryExists(string path);

        void DeleteFile(string path);

        void DeleteDirectory(string path, bool recursive = true);

        void MoveFile(string sourcePath, string destPath);

        void EnsureDirectory(string path);

        void ReplaceFile(string sourcePath, string destPath, string destBackupPath);

        IEnumerable<string> EnumerateFiles(string path, string pattern);

        IEnumerable<string> EnumerateDirectories(string path);

        Stream GetStream(string path, FileMode fileMode, FileAccess fileAccess);

        long GetFileSize(string path);

        DateTime GetLastWriteTimeUtc(string path);

        IFileInfo GetFileInfo(string path);
        IFilePath Path { get; }
        void FileCopy(string sourceFile, string destinationFile);
        IEnumerable<string> GetDirectories(string directoryPath);
        IEnumerable<string> DirectoryGetFiles(string outputFolderPath);
       
    }
}