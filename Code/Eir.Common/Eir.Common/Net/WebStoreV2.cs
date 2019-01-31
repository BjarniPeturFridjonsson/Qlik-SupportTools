using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Eir.Common.Common;
using Eir.Common.Net.Http;
using Eir.Common.Rest;
using Newtonsoft.Json;

namespace Eir.Common.Net
{
    public abstract class WebStoreV2<T> : WebStore
        where T : class
    {
        protected WebStoreV2(BaseUris baseUris, string userAgent)
            : base(2, baseUris, userAgent)
        {
        }

        protected Task<ResponseObject<T>> Get(UriFragment uriFragment, Trying trying = null, CancellationToken ct = default(CancellationToken))
        {
            return Request<T>(
                uriFragment,
                (webClient, uri) => webClient.DownloadStringAsync(uri, ct),
                trying);
        }

        protected Task<ResponseObject<TValue>> Get<TValue>(UriFragment uriFragment, Trying trying = null, CancellationToken ct = default(CancellationToken))
        {
            return Request<TValue>(
                uriFragment,
                (webClient, uri) => webClient.DownloadStringAsync(uri, ct),
                trying);
        }

        protected Task<ResponseObject<IEnumerable<T>>> GetAll(UriFragment uriFragment, Trying trying = null, CancellationToken ct = default(CancellationToken))
        {
            return Request<IEnumerable<T>>(
                uriFragment,
                (webClient, uri) => webClient.DownloadStringAsync(uri, ct),
                trying);
        }

        protected Task<ResponseObject<IEnumerable<TValue>>> GetAll<TValue>(UriFragment uriFragment, Trying trying = null, CancellationToken ct = default(CancellationToken))
        {
            return Request<IEnumerable<TValue>>(
                uriFragment,
                (webClient, uri) => webClient.DownloadStringAsync(uri, ct),
                trying);
        }

        protected Task<ResponseObject<T>> Post(UriFragment uriFragment, T item, Trying trying = null, CancellationToken ct = default(CancellationToken))
        {
            return Request<T>(
                uriFragment,
                (webClient, uri) => webClient.UploadStringAsync(uri, HttpMethod.Post, JsonConvert.SerializeObject(item), ct),
                trying);
        }

        protected Task<ResponseObject<IEnumerable<T>>> PostMany(UriFragment uriFragment, IEnumerable<T> items, Trying trying = null, CancellationToken ct = default(CancellationToken))
        {
            return Request<IEnumerable<T>>(
                uriFragment,
                (webClient, uri) => webClient.UploadStringAsync(uri, HttpMethod.Post, JsonConvert.SerializeObject(items), ct),
                trying);
        }

        protected Task<ResponseObject<T>> Put(UriFragment uriFragment, T item, Trying trying = null, CancellationToken ct = default(CancellationToken))
        {
            return Request<T>(
                uriFragment,
                (webClient, uri) => webClient.UploadStringAsync(uri, HttpMethod.Put, JsonConvert.SerializeObject(item), ct),
                trying);
        }

        protected Task<ResponseObject<TValue>> Put<TValue>(UriFragment uriFragment, TValue item, Trying trying = null, CancellationToken ct = default(CancellationToken))
        {
            return Request<TValue>(
                uriFragment,
                (webClient, uri) => webClient.UploadStringAsync(uri, HttpMethod.Put, JsonConvert.SerializeObject(item), ct),
                trying);
        }

        protected Task<ResponseObject<T>> Delete(UriFragment uriFragment, T item, Trying trying = null, CancellationToken ct = default(CancellationToken))
        {
            return Request<T>(
                uriFragment,
                (webClient, uri) => webClient.UploadStringAsync(uri, HttpMethod.Delete, JsonConvert.SerializeObject(item), ct),
                trying);
        }

        protected Task<ResponseObject<int>> DeleteMany(UriFragment uriFragment, IEnumerable<T> item, Trying trying = null, CancellationToken ct = default(CancellationToken))
        {
            return Request<int>(
                uriFragment,
                (webClient, uri) => webClient.UploadStringAsync(uri, HttpMethod.Delete, JsonConvert.SerializeObject(item), ct),
                trying);
        }

        private Task<ResponseObject<TResult>> Request<TResult>(
            UriFragment uriFragment,
            Func<IEirWebClient, Uri, Task<string>> webFunction,
            Trying trying,
            [CallerMemberName] string memberName = null)
        {
            return Request(
                uriFragment,
                async (webClient, uri) =>
                {
                    string content = await webFunction(webClient, uri).ConfigureAwait(false);

                    ResponseObject<TResult> deserializedResponse;

                    // First make an attempt to deserialize the response according to V2 standards (as a response object)
                    if (!TryDeserializeResponseObject(content, out deserializedResponse))
                    {
                        // ... and if that fails, fall back to deserializing the old way.
                        deserializedResponse = GetResponseObjectFromOldStyleContent<TResult>(content);
                    }

                    if (deserializedResponse?.Errors.Any() ?? false)
                    {
                        throw new RestException(deserializedResponse.Errors);
                    }

                    return deserializedResponse;
                },
                trying,
                null,
                // ReSharper disable once ExplicitCallerInfoArgument
                memberName);
        }

        private static bool TryDeserializeResponseObject<TResult>(string content, out ResponseObject<TResult> deserializedResponse)
        {
            try
            {
                deserializedResponse = JsonConvert.DeserializeObject<ResponseObject<TResult>>(content);
                return true;
            }
            catch
            {
                deserializedResponse = null;
                return false;
            }
        }

        private static ResponseObject<TResult> GetResponseObjectFromOldStyleContent<TResult>(string content)
        {
            TResult value = JsonConvert.DeserializeObject<TResult>(content);
            return new ResponseObject<TResult>(value, null, null);
        }
    }
}