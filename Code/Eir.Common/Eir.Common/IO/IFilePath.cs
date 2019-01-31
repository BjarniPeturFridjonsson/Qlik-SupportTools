namespace Eir.Common.IO
{
    public interface IFilePath
    {
        string Combine(params string[] paths);
        string GetFileNameWithoutExtension(string path);
        string GetExtension(string path);
        string GetFileName(string path);
        string GetTempFileName();
        string GetTempPath();
    }
}
