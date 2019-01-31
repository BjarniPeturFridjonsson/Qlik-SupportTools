namespace Eir.Common.IO
{
    public class FilePath : IFilePath
    {
        public string Combine(params string[] paths)
        {
            return System.IO.Path.Combine(paths);
        }

        public string GetFileNameWithoutExtension(string path)
        {
            return System.IO.Path.GetFileNameWithoutExtension(path);
        }

        public string GetExtension(string path)
        {
            return System.IO.Path.GetExtension(path);
        }

        public string GetFileName(string path)
        {
            return System.IO.Path.GetFileName(path);
        }

        public string GetTempFileName()
        {
            return System.IO.Path.GetTempFileName();
        }

        public string GetTempPath()
        {
            return System.IO.Path.GetTempPath();
        }
    }
}
