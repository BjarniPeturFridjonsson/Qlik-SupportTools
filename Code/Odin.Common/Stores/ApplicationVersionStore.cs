using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bifrost.Model.Models;
using Eir.Common.Net;

namespace Odin.Common.Stores
{
    public interface IApplicationVersionStore
    {
        Task<ApplicationVersion> GetLatest(Guid applicationId);
        Task<IEnumerable<ApplicationVersion>> GetAll(Guid applicationId);
        Task<bool> Add(ApplicationVersion item);
    }

    internal class ApplicationVersionStore : WebStoreV1<ApplicationVersion>, IApplicationVersionStore
    {
        private readonly UriFragment _commonUriFragment = new UriFragment("application/version");

        public ApplicationVersionStore(BaseUris baseUris, string userAgent)
            : base(baseUris, userAgent)
        {
        }

        public Task<ApplicationVersion> GetLatest(Guid applicationId)
        {
            return GetAsync(_commonUriFragment.Append("latest", new UriArg("applicationId", applicationId)));
        }

        public Task<IEnumerable<ApplicationVersion>> GetAll(Guid applicationId)
        {
            return GetAllAsync(_commonUriFragment.Append(new UriArg("applicationId", applicationId)));
        }

        public Task<bool> Add(ApplicationVersion item)
        {
            return TryWork(() => PostAsync(_commonUriFragment, item));
        }
    }
}