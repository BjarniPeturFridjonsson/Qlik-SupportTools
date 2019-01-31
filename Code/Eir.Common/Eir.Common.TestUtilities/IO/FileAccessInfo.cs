namespace Eir.Common.IO
{
    public class FileAccessInfo
    {
        public FileAccessInfo(int reads, int writes)
        {
            Reads = reads;
            Writes = writes;
        }

        public int Reads { get; }

        public int Writes { get; }
    }
}