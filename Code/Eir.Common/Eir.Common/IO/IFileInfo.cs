using System;

namespace Eir.Common.IO
{
    public interface IFileInfo
    {
        string FullName { get; }

        string Name { get; }

        string Extension { get; }

        long Length { get; }

        DateTime LastWriteTimeUtc { get; }
        DateTime CreationTime { get; }
        DateTime LastWriteTime { get; }
        void Refresh();
    }
}
