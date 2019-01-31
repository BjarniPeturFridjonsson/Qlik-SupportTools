namespace Eir.Common.Logging
{
    public interface ILogItemHandler<in TLogItem>
        where TLogItem : LogItem
    {
        void Add(TLogItem logItem);
    }
}