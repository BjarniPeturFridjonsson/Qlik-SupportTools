using System.Collections.Generic;

namespace Eir.Common.IO
{
    public interface IDirectoryInfo
    {
        IEnumerable<IFileInfo> GetFiles(string path);
    }
}
