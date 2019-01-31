namespace FreyrCommon.Logging
{
    public interface ILoggerFactory
    {
        ILogger Create<T>();

        ILogger Create(string name);
    }
}