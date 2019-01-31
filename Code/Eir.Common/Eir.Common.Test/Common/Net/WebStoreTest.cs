using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Eir.Common.Common;
using Eir.Common.Net;
using Eir.Common.Net.Http;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Eir.Common.Test.Common.Net
{
    [TestFixture]
    public class WebStoreTest : TestBase
    {
        class TestWebStore : WebStore
        {
            public TestWebStore(int apiVersion, BaseUris baseUris, string userAgent) : base(apiVersion, baseUris, userAgent)
            {
            }

            public Task<string> GetString()
            {
                return Request(new UriFragment("string"), (client, uri) => GetWebClient().DownloadStringAsync(uri, CancellationToken.None), Trying.Once, null);
            }

            public Mock<IEirWebClient> WebClientMock { get; } = new Mock<IEirWebClient>();

            protected override IEirWebClient GetWebClient()
            {
                return WebClientMock.Object;
            }
        }


        [Test]
        public async Task FailedCallQuaranteensUrl_OneUri()
        {
            var callCount = 0;
            var webStore = new TestWebStore(2, new BaseUris(new[] { "http://localhost" }, MultiUriSelectionStrategyFactory.Default), "test user agent");

            webStore.WebClientMock
                .Setup(x => x.DownloadStringAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
                    if (callCount++ == 0)
                    {
                        // the first call fails with a name resolution failure
                        tcs.TrySetException(new WebException("Server not found", WebExceptionStatus.NameResolutionFailure));
                    }
                    else
                    {
                        tcs.TrySetResult("Downloaded string");
                    }

                    return tcs.Task;
                });

            // first call fails on name resolution, so URL should be quaranteened for some time
            await webStore.GetString().ShouldThrowAsync<AggregateException>();

            // second call should not fail...
            (await webStore.GetString()).ShouldBe("Downloaded string");

            // since the second call should fail on the URL being quaranteened, the second call should not even be attempted
            callCount.ShouldBe(2);
        }

        [Test]
        public async Task FailedCallQuaranteensUrl_TwoUris()
        {
            var callCount = 0;
            var webStore = new TestWebStore(2, new BaseUris(new[] { "http://localhost", "http://localhost2" }, MultiUriSelectionStrategyFactory.Default), "test user agent");

            webStore.WebClientMock
                .Setup(x => x.DownloadStringAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Returns<Uri, CancellationToken>((uri, ct) =>
                {
                    callCount++;

                    TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
                    if (uri.Host == "localhost")
                    {
                        tcs.TrySetException(new WebException("Server not found", WebExceptionStatus.NameResolutionFailure));
                    }
                    else if (uri.Host == "localhost2")
                    {
                        tcs.TrySetResult("Downloaded string");
                    }
                    else
                    {
                        throw new Exception("Unexpected host! :-O");
                    }

                    return tcs.Task;
                });

            // It should work...
            (await webStore.GetString()).ShouldBe("Downloaded string");

            // ...but two calls should be made
            callCount.ShouldBe(2);
        }

        [Test]
        public async Task FailedCallQuaranteensUrl_TwoUris_FirstRoundFails_SecondRoundWorks()
        {
            var callCount = 0;
            var webStore = new TestWebStore(2, new BaseUris(new[] { "http://localhost", "http://localhost2" }, MultiUriSelectionStrategyFactory.Default), "test user agent");

            bool serverFound = true;

            webStore.WebClientMock
                .Setup(x => x.DownloadStringAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Returns<Uri, CancellationToken>((uri, ct) =>
                {
                    callCount++;
                    TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
                    if (serverFound)
                    {
                        tcs.TrySetResult("Downloaded string");
                    }
                    else
                    {
                        tcs.TrySetException(new WebException("Server not found", WebExceptionStatus.NameResolutionFailure));
                    }
                    return tcs.Task;
                });

            serverFound = false;
            await webStore.GetString().ShouldThrowAsync<AggregateException>();
            callCount.ShouldBe(2);

            callCount = 0;
            serverFound = true;
            (await webStore.GetString()).ShouldBe("Downloaded string");
            callCount.ShouldBe(1);
        }

        [Test]
        public async Task HttpFailedCallDoesNotQuaranteenUrl()
        {
            var callCount = 0;
            var webStore = new TestWebStore(2, new BaseUris(new[] { "http://localhost" }, MultiUriSelectionStrategyFactory.Default), "test user agent");

            webStore.WebClientMock
                .Setup(x => x.DownloadStringAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
                    if (callCount++ == 0)
                    {
                        // the first call fails with a protocol error (i e an HTTP error response)
                        tcs.TrySetException(new WebException("Resource not found", WebExceptionStatus.ProtocolError));
                    }
                    else
                    {
                        tcs.TrySetResult("Downloaded string");
                    }

                    return tcs.Task;
                });

            // first call fails on name resolution, so URL should be quaranteened for some time
            await webStore.GetString().ShouldThrowAsync<AggregateException>();

            // second call should not fail...
            (await webStore.GetString()).ShouldBe("Downloaded string");

            // ...and two calls should be made
            callCount.ShouldBe(2);
        }
    }
}