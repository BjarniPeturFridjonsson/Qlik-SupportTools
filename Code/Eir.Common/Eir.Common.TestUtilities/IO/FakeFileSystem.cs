using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Eir.Common.Common;
using Eir.Common.Extensions;
using NUnit.Framework;

namespace Eir.Common.IO
{
    public class FakeFileSystem : IFileSystem
    {
        private class FakeFile : IFakeFile
        {
            private readonly IDateTimeProvider _dateTimeProvider;
            private readonly List<FakeFileStream> _openStreams = new List<FakeFileStream>();

            private class FakeFileStream : Stream
            {
                private readonly MemoryStream _memoryStream;
                private readonly Action _isWritten;
                private readonly Action<byte[]> _flush;
                private readonly Action<FakeFileStream> _isClosed;

                public FakeFileStream(
                    byte[] content,
                    FileMode fileMode,
                    FileAccess fileAccess,
                    FileShare fileShare,
                    Action isWritten,
                    Action<byte[]> flush,
                    Action<FakeFileStream> isClosed)
                {
                    _memoryStream = new MemoryStream();
                    _memoryStream.Write(content, 0, content.Length);

                    FileAccess = fileAccess;
                    FileShare = fileShare;
                    _isWritten = isWritten;
                    _flush = flush;
                    _isClosed = isClosed;

                    switch (fileMode)
                    {
                        case FileMode.CreateNew:
                            _memoryStream.Position = 0;
                            break;
                        case FileMode.Create:
                            _memoryStream.Position = 0;
                            break;
                        case FileMode.Open:
                            _memoryStream.Position = 0;
                            break;
                        case FileMode.OpenOrCreate:
                            _memoryStream.Position = 0;
                            break;
                        case FileMode.Truncate:
                            _memoryStream.SetLength(0);
                            break;
                        case FileMode.Append:
                            _memoryStream.Position = _memoryStream.Length;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(fileMode), fileMode, null);
                    }
                }

                public FileAccess FileAccess { get; }

                public FileShare FileShare { get; }

                public override void Flush()
                {
                    _flush(_memoryStream.ToArray());
                }

                public override long Seek(long offset, SeekOrigin origin)
                {
                    if (!CanSeek) throw new NotSupportedException();
                    return _memoryStream.Seek(offset, origin);
                }

                public override void SetLength(long value)
                {
                    if (!CanWrite) throw new NotSupportedException();
                    _memoryStream.SetLength(value);
                    _isWritten();
                }

                public override int Read(byte[] buffer, int offset, int count)
                {
                    if (!CanRead) throw new NotSupportedException();
                    int readCount = _memoryStream.Read(buffer, offset, count);
                    return readCount;
                }

                public override void Write(byte[] buffer, int offset, int count)
                {
                    if (!CanWrite) throw new NotSupportedException();
                    _memoryStream.Write(buffer, offset, count);
                    _isWritten();
                }

                public override bool CanRead => FileAccess != FileAccess.Write;

                public override bool CanSeek => true;

                public override bool CanWrite => FileAccess != FileAccess.Read;

                public override long Length => _memoryStream.Length;

                public override long Position
                {
                    get { return _memoryStream.Position; }
                    set
                    {
                        if (!CanSeek) throw new NotSupportedException();
                        _memoryStream.Position = value;
                    }
                }

                public override void Close()
                {
                    base.Close();
                    Flush();
                    _memoryStream.Close();
                    _isClosed(this);
                }

                protected override void Dispose(bool disposing)
                {
                    base.Dispose(disposing);
                    Flush();
                    _isClosed(this);
                }
            }

            public FakeFile(string path, IDateTimeProvider dateTimeProvider)
            {
                _dateTimeProvider = dateTimeProvider;
                FullName = path;
                LastWriteTimeUtc = dateTimeProvider.Time();
            }

            public string FullName { get; }

            public long Length => ContentAsBytes.Length;

            public DateTime LastWriteTimeUtc { get; set; }

            public void ValidateAccess(FileShare fileShare)
            {
                if (Locked)
                {
                    throw new AccessViolationException();
                }

                switch (fileShare)
                {
                    case FileShare.None:
                        if (_openStreams.Any()) throw new AccessViolationException();
                        break;

                    case FileShare.Read:
                        if (_openStreams.Any(x => (x.FileAccess != FileAccess.Read) || (x.FileShare == FileShare.None))) throw new AccessViolationException();
                        break;

                    case FileShare.Write:
                        if (_openStreams.Any(x => (x.FileAccess != FileAccess.Write) || (x.FileShare == FileShare.None))) throw new AccessViolationException();
                        break;

                    case FileShare.ReadWrite:
                        if (_openStreams.Any(x => x.FileShare == FileShare.None)) throw new AccessViolationException();
                        break;

                    case FileShare.Delete:
                        if (_openStreams.Any()) throw new AccessViolationException();
                        break;

                    case FileShare.Inheritable:
                        throw new NotSupportedException();

                    default:
                        throw new ArgumentOutOfRangeException(nameof(fileShare), fileShare, null);
                }
            }

            public Stream GetStream(FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
            {
                ValidateAccess(fileShare);

                switch (fileShare)
                {
                    case FileShare.None:
                        if (_openStreams.Any()) throw new AccessViolationException();
                        break;

                    case FileShare.Read:
                        if (_openStreams.Any(x => (x.FileAccess != FileAccess.Read) || (x.FileShare == FileShare.None))) throw new AccessViolationException();
                        break;

                    case FileShare.Write:
                        if (_openStreams.Any(x => (x.FileAccess != FileAccess.Write) || (x.FileShare == FileShare.None))) throw new AccessViolationException();
                        break;

                    case FileShare.ReadWrite:
                        if (_openStreams.Any(x => x.FileShare == FileShare.None)) throw new AccessViolationException();
                        break;

                    case FileShare.Delete:
                        throw new NotSupportedException();

                    case FileShare.Inheritable:
                        throw new NotSupportedException();

                    default:
                        throw new ArgumentOutOfRangeException(nameof(fileShare), fileShare, null);
                }

                var stream = new FakeFileStream(
                    ContentAsBytes,
                    fileMode,
                    fileAccess,
                    fileShare,
                    () => LastWriteTimeUtc = _dateTimeProvider.Time(),
                    flushedContent =>
                    {
                        switch (fileAccess)
                        {
                            case FileAccess.Write:
                            case FileAccess.ReadWrite:
                                ContentAsBytes = flushedContent;
                                break;
                        }
                    },
                    s => _openStreams.Remove(s));

                _openStreams.Add(stream);

                return stream;
            }

            public byte[] ContentAsBytes { get; set; } = new byte[0];

            public string ContentAsUtf8String
            {
                get
                {
                    foreach (FakeFileStream fakeFileStream in _openStreams)
                    {
                        fakeFileStream.Flush();
                    }

                    if (ContentAsBytes.Length == 0)
                    {
                        return string.Empty;
                    }

                    using (var memoryStream = new MemoryStream(ContentAsBytes))
                    {
                        using (var reader = new StreamReader(memoryStream, Encoding.UTF8))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
                set
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))
                        {
                            writer.Write(value);
                        }

                        ContentAsBytes = memoryStream.ToArray();
                    }
                }
            }

            public string ContentAsHex
            {
                get { return string.Concat(ContentAsBytes.Select(x => x.ToString("x2"))); }
                set
                {
                    try
                    {
                        if ((value.Length % 2) != 0)
                        {
                            throw new FormatException("Bad length.");
                        }

                        Func<string, int, byte> subStringToByte = (text, index) => byte.Parse(text.Substring(index, 2), NumberStyles.HexNumber);

                        int byteCount = value.Length / 2;
                        var bytes = new byte[byteCount];
                        for (int i = 0; i < byteCount; i++)
                        {
                            bytes[i] = subStringToByte(value, i * 2);
                        }

                        ContentAsBytes = bytes;
                    }
                    catch (Exception exception)
                    {
                        throw new FormatException("Not a valid HEX string", exception);
                    }
                }
            }

            public string ContentAsBase64
            {
                get { return Convert.ToBase64String(ContentAsBytes); }
                set { ContentAsBytes = Convert.FromBase64String(value); }
            }

            public bool Locked { get; set; }

            public override string ToString() => $"{FullName} ({ContentAsBytes.Length} bytes)";
        }

        private readonly List<FakeFile> _files = new List<FakeFile>();
        private readonly HashSet<string> _knownDirs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly bool _verboseLog;
        private readonly Dictionary<string, FileAccessInfo> _fileAccessInfos = new Dictionary<string, FileAccessInfo>(StringComparer.OrdinalIgnoreCase);

        public FakeFileSystem(IDateTimeProvider dateTimeProvider, bool verboseLog = false)
        {
            _dateTimeProvider = dateTimeProvider;
            _verboseLog = verboseLog;
        }

        public IFakeFile SetFileFromBytes(string path, byte[] content)
        {
            IFakeFile file = SetFile(path);
            file.ContentAsBytes = content;
            return file;
        }

        public IFakeFile SetFileFromUtf8String(string path, string content)
        {
            IFakeFile file = SetFile(path);
            file.ContentAsUtf8String = content;
            return file;
        }

        public IFakeFile SetFileFromHex(string path, string hexContent)
        {
            IFakeFile file = SetFile(path);
            file.ContentAsHex = hexContent;
            return file;
        }

        public IFakeFile SetFileFromBase64(string path, string base64Content)
        {
            IFakeFile file = SetFile(path);
            file.ContentAsBase64 = base64Content;
            return file;
        }

        private IFakeFile SetFile(string path)
        {
            _files.RemoveAll(x => x.FullName.Equals(path, StringComparison.OrdinalIgnoreCase));
            return InternalGetFile(path, MissingFileStrategy.CreateNew);
        }

        public IFakeFile GetFile(string path)
        {
            return InternalGetFile(path, MissingFileStrategy.ReturnNull);
        }

        private enum MissingFileStrategy
        {
            ReturnNull,
            CreateNew,
            ThrowException
        }

        private FakeFile InternalGetFile(string path, MissingFileStrategy missingFileStrategy)
        {
            var file = _files.FirstOrDefault(x => x.FullName.Equals(path, StringComparison.CurrentCultureIgnoreCase));
            if (file == null)
            {
                switch (missingFileStrategy)
                {
                    case MissingFileStrategy.ReturnNull:
                        break;

                    case MissingFileStrategy.CreateNew:
                        file = new FakeFile(path, _dateTimeProvider);
                        _files.Add(file);
                        break;

                    case MissingFileStrategy.ThrowException:
                        throw new FileNotFoundException();

                    default:
                        throw new ArgumentOutOfRangeException(nameof(missingFileStrategy), missingFileStrategy, null);
                }
            }

            return file;
        }

        private void InternalDeleteFile(FakeFile file)
        {
            if (file == null)
            {
                return;
            }

            file.ValidateAccess(FileShare.Delete);
            _files.Remove(file);
        }

        private static string CleanDir(string path)
        {
            if (path.EndsWith(@"\"))
            {
                path = path.Substring(0, path.Length - 1);
            }
            return path;
        }

        public FileAccessInfo GetFileAccessInfo(string path)
        {
            FileAccessInfo fileAccessInfo;
            return !_fileAccessInfos.TryGetValue(path, out fileAccessInfo)
                ? new FileAccessInfo(0, 0)
                : fileAccessInfo;
        }

        private void UpdateFileAccessInfo(string path, FileAccess fileAccess)
        {
            FileAccessInfo fileAccessInfo = GetFileAccessInfo(path);

            switch (fileAccess)
            {
                case FileAccess.Read:
                    fileAccessInfo = new FileAccessInfo(fileAccessInfo.Reads + 1, fileAccessInfo.Writes);
                    break;

                case FileAccess.Write:
                    fileAccessInfo = new FileAccessInfo(fileAccessInfo.Reads, fileAccessInfo.Writes + 1);
                    break;

                case FileAccess.ReadWrite:
                    fileAccessInfo = new FileAccessInfo(fileAccessInfo.Reads + 1, fileAccessInfo.Writes + 1);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(fileAccess), fileAccess, null);
            }

            _fileAccessInfos[path] = fileAccessInfo;
        }

        private void VerboseDebugLog(string text)
        {
            if (_verboseLog)
            {
                Console.WriteLine(text);
            }
        }

        bool IFileSystem.FileExists(string path)
        {
            VerboseDebugLog($"{nameof(IFileSystem.FileExists)}(\"{path}\")");

            return GetFile(path) != null;
        }

        bool IFileSystem.DirectoryExists(string path)
        {
            VerboseDebugLog($"{nameof(IFileSystem.DirectoryExists)}(\"{path}\")");

            string dir = CleanDir(path);

            if (_files.Any(x => x.FullName.StartsWith(dir, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            return _knownDirs.Any(x => x.StartsWith(dir, StringComparison.OrdinalIgnoreCase));
        }

        void IFileSystem.DeleteFile(string path)
        {
            VerboseDebugLog($"{nameof(IFileSystem.DeleteFile)}(\"{path}\")");

            FakeFile file = InternalGetFile(path, MissingFileStrategy.ThrowException);
            InternalDeleteFile(file);
        }

        void IFileSystem.DeleteDirectory(string path, bool recursive)
        {
            VerboseDebugLog($"{nameof(IFileSystem.DeleteDirectory)}(\"{path}\", {recursive})");

            string dir = CleanDir(path);

            if (!recursive)
            {
                // Check existing sub items -- and throw exception if anyone is found!
            }

            foreach (FakeFile file in _files)
            {
                if (file.FullName.StartsWith(dir, StringComparison.OrdinalIgnoreCase))
                {
                    if (!recursive)
                    {
                        throw new IOException("The directory is not empty.");
                    }

                    file.ValidateAccess(FileShare.Delete);
                }
            }

            if (!recursive)
            {
                foreach (string knownDir in _knownDirs)
                {
                    if (knownDir.StartsWith(dir, StringComparison.OrdinalIgnoreCase) && (knownDir.Length > dir.Length))
                    {
                        throw new IOException("The directory is not empty.");
                    }
                }
            }

            _files.RemoveAll(x => x.FullName.StartsWith(dir, StringComparison.OrdinalIgnoreCase));

            foreach (var knownDir in _knownDirs.Where(x => x.StartsWith(dir, StringComparison.OrdinalIgnoreCase)).ToArray())
            {
                _knownDirs.Remove(knownDir);
            }
        }

        void IFileSystem.MoveFile(string sourcePath, string destPath)
        {
            VerboseDebugLog($"{nameof(IFileSystem.MoveFile)}(\"{sourcePath}\", {destPath})");

            FakeFile sourceFile = InternalGetFile(sourcePath, MissingFileStrategy.ThrowException);
            FakeFile destFile = InternalGetFile(destPath, MissingFileStrategy.ReturnNull);
            if (destFile != null)
            {
                throw new IOException("Existing file!");
            }

            byte[] content = sourceFile.ContentAsBytes;
            InternalDeleteFile(sourceFile);
            InternalGetFile(destPath, MissingFileStrategy.CreateNew).ContentAsBytes = content;
        }

        void IFileSystem.EnsureDirectory(string path)
        {
            VerboseDebugLog($"{nameof(IFileSystem.EnsureDirectory)}(\"{path}\")");

            _knownDirs.Add(CleanDir(path));
        }

        void IFileSystem.ReplaceFile(string sourcePath, string destPath, string destBackupPath)
        {
            VerboseDebugLog($"{nameof(IFileSystem.ReplaceFile)}(\"{sourcePath}\", \"{destPath}\", \"{destBackupPath}\")");

            FakeFile sourceFile = InternalGetFile(sourcePath, MissingFileStrategy.ThrowException);

            this.CopyAsync(destPath, destBackupPath, true).SafeWait();
            this.CopyAsync(sourcePath, destPath, true).SafeWait();
            InternalDeleteFile(sourceFile);
        }

        IEnumerable<string> IFileSystem.EnumerateFiles(string path, string pattern)
        {
            VerboseDebugLog($"{nameof(IFileSystem.EnumerateFiles)}(\"{path}\", \"{pattern}\")");

            string dir = CleanDir(path);

            Regex regex = PatternToRegex(pattern);

            var matches = new List<string>();

            foreach (FakeFile file in _files)
            {
                string fileDir = Path.GetDirectoryName(file.FullName);
                if (dir.Equals(fileDir, StringComparison.OrdinalIgnoreCase))
                {
                    if (regex.IsMatch(Path.GetFileName(file.FullName)))
                    {
                        matches.Add(file.FullName);
                    }
                }
            }

            matches.Sort(StringComparer.OrdinalIgnoreCase);
            return matches;
        }

        IEnumerable<string> IFileSystem.EnumerateDirectories(string path)
        {
            VerboseDebugLog($"{nameof(IFileSystem.EnumerateDirectories)}(\"{path}\")");

            string dir = CleanDir(path);

            var matches = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string knownDir in _files
                .Select(file => Path.GetDirectoryName(file.FullName) ?? string.Empty)
                .Concat(_knownDirs))
            {
                if (knownDir.StartsWith(dir, StringComparison.OrdinalIgnoreCase))
                {
                    string subDirs = knownDir.Substring(dir.Length).Trim('\\');
                    if (!string.IsNullOrEmpty(subDirs))
                    {
                        var firstSubDir = subDirs.Split('\\')[0];
                        matches.Add(Path.Combine(dir, firstSubDir));
                    }
                }
            }

            return matches.OrderBy(x => x);
        }

        private static Regex PatternToRegex(string pattern)
        {
            // fix '.' -> '\.'
            // fix '*' -> '.*'
            // fix '?' -> '.{1,1}'

            return new Regex(
                "^" +
                pattern
                    .Replace(".", "\\.")
                    .Replace("*", ".*")
                    .Replace("?", ".{1,1}") +
               "$");
        }

        Stream IFileSystem.GetStream(string path, FileMode fileMode, FileAccess fileAccess)
        {
            VerboseDebugLog($"{nameof(IFileSystem.GetStream)}(\"{path}\", FileMode.{fileMode}, FileAccess.{fileAccess})");

            UpdateFileAccessInfo(path, fileAccess);

            FakeFile file;
            switch (fileMode)
            {
                case FileMode.CreateNew:
                    file = InternalGetFile(path, MissingFileStrategy.ReturnNull);
                    if (file != null)
                    {
                        throw new IOException("Already exists");
                    }

                    file = InternalGetFile(path, MissingFileStrategy.CreateNew);
                    break;

                case FileMode.Create:
                    file = InternalGetFile(path, MissingFileStrategy.CreateNew);
                    if (file != null)
                    {
                        InternalDeleteFile(file);
                    }

                    file = InternalGetFile(path, MissingFileStrategy.CreateNew);
                    break;

                case FileMode.Open:
                    file = InternalGetFile(path, MissingFileStrategy.ThrowException);
                    break;

                case FileMode.OpenOrCreate:
                    file = InternalGetFile(path, MissingFileStrategy.CreateNew);
                    break;

                case FileMode.Truncate:
                    file = InternalGetFile(path, MissingFileStrategy.ThrowException);
                    break;

                case FileMode.Append:
                    file = InternalGetFile(path, MissingFileStrategy.CreateNew);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(fileMode), fileMode, null);
            }

            return file.GetStream(fileMode, fileAccess, FileShare.Read);
        }

        long IFileSystem.GetFileSize(string path)
        {
            VerboseDebugLog($"{nameof(IFileSystem.GetFileSize)}(\"{path}\")");

            return InternalGetFile(path, MissingFileStrategy.ThrowException).Length;
        }

        DateTime IFileSystem.GetLastWriteTimeUtc(string path)
        {
            VerboseDebugLog($"{nameof(IFileSystem.GetLastWriteTimeUtc)}(\"{path}\")");

            return InternalGetFile(path, MissingFileStrategy.ThrowException).LastWriteTimeUtc;
        }

        public IFileInfo GetFileInfo(string path)
        {
            return InternalGetFile(path, MissingFileStrategy.ThrowException);
        }

        public void AssertFileList(params string[] paths)
        {
            var missingFiles = paths.Where(path => InternalGetFile(path, MissingFileStrategy.ReturnNull) == null).ToArray();
            var unwantedFiles = _files.Select(file => file.FullName).Where(path => paths.All(p => !p.Equals(path, StringComparison.OrdinalIgnoreCase))).ToArray();

            foreach (string path in missingFiles)
            {
                Console.WriteLine($"Missing file: \"{path}\"");
            }

            foreach (string path in unwantedFiles)
            {
                Console.WriteLine($"Unwanted file: \"{path}\"");
            }

            if (missingFiles.Any() || unwantedFiles.Any())
            {
                Assert.Fail("Unexpected state of the file.");
            }
        }

        public void PrintToConsole()
        {
            Console.WriteLine("====== FILE SYSTEM ======");
            foreach (var file in _files.OrderBy(x => x.FullName, StringComparer.OrdinalIgnoreCase))
            {
                Console.WriteLine(file);
            }
            Console.WriteLine("=========================");
        }
    }
}