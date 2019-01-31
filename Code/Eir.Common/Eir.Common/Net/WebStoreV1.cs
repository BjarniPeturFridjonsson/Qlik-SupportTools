using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eir.Common.Common;
using Eir.Common.Net.Http;
using Newtonsoft.Json;

namespace Eir.Common.Net
{
    public abstract class WebStoreV1<T> : WebStore
        where T : class
    {
        protected WebStoreV1(BaseUris baseUris, string userAgent)
            : base(1, baseUris, userAgent)
        {
        }

        protected IEnumerable<T> GetAll(UriFragment uriFragment, Trying trying = null, Func<IList<T>> getResultIfFail = null)
        {
            return Request(
                uriFragment,
                (webClient, uri) =>
                {
                    string content = webClient.DownloadString(uri);
                    return Task.FromResult(JsonConvert.DeserializeObject<IEnumerable<T>>(content));
                },
                trying,
                getResultIfFail).Result;
        }

        protected Task<IEnumerable<T>> GetAllAsync(UriFragment uriFragment, Trying trying = null, Func<IEnumerable<T>> getResultIfFail = null, CancellationToken ct = default(CancellationToken))
        {
            return Request(
                uriFragment,
                async (webClient, uri) =>
                {
                    string content = await webClient.DownloadStringAsync(uri, ct).ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<IEnumerable<T>>(content);
                },
                trying,
                getResultIfFail);
        }

        protected T Get(UriFragment uriFragment, Trying trying = null, Func<T> getResultIfFail = null)
        {
            return Request(
                uriFragment,
                (webClient, uri) =>
                {
                    string content = webClient.DownloadString(uri);
                    return Task.FromResult(JsonConvert.DeserializeObject<T>(content));
                },
                trying,
                getResultIfFail).Result;
        }

        protected Task<T> GetAsync(UriFragment uriFragment, Trying trying = null, Func<T> getResultIfFail = null, CancellationToken ct = default(CancellationToken))
        {
            return Request(
                uriFragment,
                async (webClient, uri) =>
                {
                    string content = await webClient.DownloadStringAsync(uri, ct).ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<T>(content);
                },
                trying,
                getResultIfFail);
        }

        protected Task<TValue> GetAsync<TValue>(UriFragment uriFragment, Trying trying = null, Func<TValue> getResultIfFail = null, CancellationToken ct = default(CancellationToken))
        {
            return Request(
                uriFragment,
                async (webClient, uri) =>
                {
                    string content = await webClient.DownloadStringAsync(uri, ct).ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<TValue>(content);
                },
                trying,
                getResultIfFail);
        }

        protected Task<bool> DeleteAsync(UriFragment uriFragment, T item, Trying trying = null, CancellationToken ct = default(CancellationToken))
        {
            string json = JsonConvert.SerializeObject(item);

            return Request(
                uriFragment,
                async (webClient, uri) =>
                {
                    await webClient.UploadDataAsync(uri, HttpMethod.Delete, Encoding.UTF8.GetBytes(json), ct).ConfigureAwait(false);
                    return true;
                },
                trying,
                () => false /* Return-value if failed. */); // TODO: <<--  Throw on delete instead of returning false!
        }

        protected async Task<string> PutAsStringAsync(UriFragment uriFragment, T item, Trying trying = null, CancellationToken ct = default(CancellationToken))
        {
            string json = JsonConvert.SerializeObject(item);

            return await Request(
                uriFragment,
                async (webClient, uri) =>
                {
                    byte[] response = await webClient.UploadDataAsync(uri, HttpMethod.Put, Encoding.UTF8.GetBytes(json), ct).ConfigureAwait(false);
                    return Encoding.UTF8.GetString(response);
                },
                trying,
                null).ConfigureAwait(false);
        }
        
        protected async Task<T> PutAsync(UriFragment uriFragment, T item, Trying trying = null, CancellationToken ct = default(CancellationToken))
        {
            string json = JsonConvert.SerializeObject(item);

            return await Request(
                uriFragment,
                async (webClient, uri) =>
                {
                    byte[] response = await webClient.UploadDataAsync(uri, HttpMethod.Put, Encoding.UTF8.GetBytes(json), ct).ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(response));
                },
                trying,
                null).ConfigureAwait(false);
        }
        
        protected async Task<T> PostAsync(UriFragment uriFragment, T item, Trying trying = null, CancellationToken ct = default(CancellationToken))
        {
            string json = JsonConvert.SerializeObject(item);

            return await Request(
                uriFragment,
                async (webClient, uri) =>
                {
                    string response = await webClient.UploadStringAsync(uri, HttpMethod.Post, json, ct).ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<T>(response);
                },
                trying,
                null).ConfigureAwait(false);
        }

        protected async Task<string> PostAsStringAsync(UriFragment uriFragment, T item, Trying trying = null, CancellationToken ct = default(CancellationToken))
        {
            string json = JsonConvert.SerializeObject(item);

            return await Request(
                uriFragment,
                async (webClient, uri) => await webClient.UploadStringAsync(uri, HttpMethod.Post, json, ct).ConfigureAwait(false),
                trying,
                null).ConfigureAwait(false);
        }

        protected async Task<bool> TryWork(Func<Task> work)
        {
            try
            {
                await work().ConfigureAwait(false);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}