using Eir.Common.IO;

namespace Gjallarhorn.SenseLogReading.FileMiners
{
    public interface IDataMiner
    {
        string MineFromThisLocation(string basePath, IFileSystem fileSystem);
        string MinerName { get; }
        void Mine(string line);
        void InitializeNewFile(string headerLine, FileMinerDto basicDataFromCase, string path);
        void FinaliseStatistics();
    }
}
