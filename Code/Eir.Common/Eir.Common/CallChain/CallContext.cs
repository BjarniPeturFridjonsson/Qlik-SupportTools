namespace Eir.Common.CallChain
{
    public interface ICallContext
    {
        IInformationLogger InformationLogger { get; }
    }

    public class CallContext : ICallContext
    {
        public CallContext(IInformationLogger informationLogger)
        {
            InformationLogger = informationLogger;
        }

        public IInformationLogger InformationLogger { get; }
    }
}