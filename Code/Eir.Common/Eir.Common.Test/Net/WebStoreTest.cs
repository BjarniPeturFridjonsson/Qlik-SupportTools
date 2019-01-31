//using System;
//using System.Net;
//using System.Threading;
//using System.Threading.Tasks;
//using Eir.Common.Common;
//using Eir.Common.Net;
//using Eir.Common.Net.Http;
//using Moq;
//using NUnit.Framework;
//using Shouldly;

//namespace Eir.Common.Test.Common.Net
//{
//    [TestFixture]
//    public class WebStoreTest
//    {
//        //class TestWebStore : WebStore
//        //{
//        //    public TestWebStore(int apiVersion, BaseUris baseUris, string userAgent) : base(apiVersion, baseUris, userAgent)
//        //    {
//        //    }

//        //    public Task<string> GetString()
//        //    {
//        //        return Request(new UriFragment("string"), (client, uri) => GetWebClient().DownloadStringAsync(uri, CancellationToken.None), Trying.Once, null);
//        //    }

//        //    public Mock<IEirWebClient> WebClientMock { get; } = new Mock<IEirWebClient>();

//        //    protected override IEirWebClient GetWebClient()
//        //    {
//        //        return WebClientMock.Object;
//        //    }
//        //}


//        [Test]
//        public async Task FailedCallQuaranteensUrl()
//        {
//            var callCount = 0;
//            var webStore = new TestWebStore(2, new BaseUris(new[] { "http://localhost" }, MultiUriSelectionStrategyFactory.Default), "test user agent");

//            webStore.WebClientMock
//                .Setup(x => x.DownloadStringAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
//                .Returns(() =>
//                {
//                    TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
//                    if (callCount++ == 0)
//                    {
//                        // the first call fails with a name resolution failure
//                        tcs.TrySetException(new WebException("Server not found", WebExceptionStatus.NameResolutionFailure));
//                    }
//                    else
//                    {
//                        tcs.TrySetResult("Downloaded string");
//                    }

//                    return tcs.Task;
//                });

//            // first call fails on name resolution, so URL should be quaranteened for some time
//            await webStore.GetString().ShouldThrowAsync<AggregateException>();

//            // second call should also fail, even though the mock is set up to be a successful call.
//            await webStore.GetString().ShouldThrowAsync<AggregateException>();

//            // since the second call should fail on the URL being quaranteened, the second call should not even be attempted
//            callCount.ShouldBe(1);
//        }

//        [Test]
//        public async Task HttpFailedCallDoesNotQuaranteenUrl()
//        {
//            var callCount = 0;
//            var webStore = new TestWebStore(2, new BaseUris(new[] { "http://localhost" }, MultiUriSelectionStrategyFactory.Default), "test user agent");

//            webStore.WebClientMock
//                .Setup(x => x.DownloadStringAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
//                .Returns(() =>
//                {
//                    TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
//                    if (callCount++ == 0)
//                    {
//                        // the first call fails with a protocol error (i e an HTTP error response)
//                        tcs.TrySetException(new WebException("Resource not found", WebExceptionStatus.ProtocolError));
//                    }
//                    else
//                    {
//                        tcs.TrySetResult("Downloaded string");
//                    }

//                    return tcs.Task;
//                });

//            // first call fails on name resolution, so URL should be quaranteened for some time
//            await webStore.GetString().ShouldThrowAsync<AggregateException>();

//            // second call should not fail...
//            (await webStore.GetString()).ShouldBe("Downloaded string");

//            // ...and two calls should be made
//            callCount.ShouldBe(2);
//        }
//    }
//}