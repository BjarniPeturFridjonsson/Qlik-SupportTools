namespace FreyrCommon.Logging
{
    public class LoggerFactory : ILoggerFactory
    {
        public ILogger Create<T>()
        {
            return new Logger(typeof(T).Name);
        }

        public ILogger Create(string name)
        {
            return new Logger(name);
        }
    }
}