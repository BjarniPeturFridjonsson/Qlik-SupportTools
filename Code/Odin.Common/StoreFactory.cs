using Eir.Common.Net;
using Odin.Common.Stores;

namespace Odin.Common
{
    public class StoreFactory : IStoreFactory
    {
        private readonly BaseUris _baseUris;

        public StoreFactory(BaseUris baseUris)
        {
            _baseUris = baseUris;
        }
        public IApplicationVersionStore GetApplicationVersionStore(string userAgent)
        {
            return new ApplicationVersionStore(_baseUris, userAgent);
        }
      
    }

}
