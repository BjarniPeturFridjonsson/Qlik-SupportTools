using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Eir.Common.IO
{
    public static class FileSystemExtensions
    {
        public static TextWriter GetWriter(this IFileSystem fileSystem, string path)
        {
            Stream stream = fileSystem.GetStream(path, FileMode.Create, FileAccess.Write);
            return new StreamWriter(stream, Encoding.UTF8);
        }

        public static TextReader GetReader(this IFileSystem fileSystem, string path)
        {
            Stream stream = fileSystem.GetStream(path, FileMode.Open, FileAccess.Read);
            return new StreamReader(stream, Encoding.UTF8);
        }

        public static async Task<string> ReadFileContentAsync(this IFileSystem fileSystem, string path)
        {
            using (var reader = GetReader(fileSystem, path))
            {
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        public static async Task CopyAsync(this IFileSystem fileSystem, string fromFilename, string toFilename, bool overwrite = false)
        {
            if (!overwrite && fileSystem.FileExists(toFilename))
            {
                throw new IOException($"The file '{toFilename}' already exists.");
            }

            using (Stream fromStream = fileSystem.GetStream(fromFilename, FileMode.Open, FileAccess.Read))
            {
                using (Stream toStream = fileSystem.GetStream(toFilename, FileMode.Create, FileAccess.Write))
                {
                    await fromStream.CopyToAsync(toStream).ConfigureAwait(false);
                }
            }
        }

        public static void AppendAllText(this IFileSystem fileSystem, string path, string text)
        {
            Stream stream = fileSystem.GetStream(path, FileMode.Append, FileAccess.Write);
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                writer.Write(text);
            }
        }

        public static string GetParentDirectory(this IFileSystem fileSystem, string path)
        {
            return Directory.GetParent(path).FullName;
        }
    }
}