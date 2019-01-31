using Odin.Common.Stores;

namespace Odin.Common
{
    public interface IStoreFactory
    {
        IApplicationVersionStore GetApplicationVersionStore(string userAgent);
    }
}
