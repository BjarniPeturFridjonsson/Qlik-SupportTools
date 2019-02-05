using System;
using System.IO;

namespace Eir.Common.Logging
{
    public class DumpLog : IDumpLog
    {
        private readonly string _dumpDir;

        public DumpLog(string dumpDir)
        {
            _dumpDir = dumpDir;
        }
         
        public void TextBlob(string data, string filename)
        {
            string path = Path.Combine(_dumpDir, filename);

            try
            {
                File.WriteAllText(path, data);
            }
            catch (Exception ex)
            {
                Log.To.Main.AddException($"Error when writing data dump file to \"{path}\".", ex);
            }
        }
    }
}