namespace Gjallarhorn.QvLogReading
{
    internal class ColumnInfo
    {
        public int Index { get; }
        public string HeaderName { get; }
        //public string DatapointName { get; }

        public ColumnInfo(int index, string headername)
        {
            Index = index;
            HeaderName = headername;
            //DatapointName = datapointname;
        }
    }
}
