using System.Diagnostics;
using NUnit.Framework;
using Eir.Common.Net;
using Shouldly;

namespace Eir.Common.Test.Common.Net
{
    [TestFixture]
    public class UriFragmentTest : TestBase
    {
        [Test]
        public void AddUriParams()
        {
            var uriFragment = new UriFragment("qlikdiagnosticsrequest");
            var a = uriFragment.Append(new UriArg("id", 123), new UriArg("myRpcCall", "rpcResource"));
            a.ToString().ShouldBe(".../qlikdiagnosticsrequest?id=123&myRpcCall=rpcResource");
        }
        
        [Test]
        public void AddUriPath() { 
            
            var uriFragment2 = new UriFragment("qlikdiagnosticsrequest");
            var a2 = uriFragment2.Append("123-123-123").Append("myRpcCall");
            a2.ToString().ShouldBe(".../qlikdiagnosticsrequest/123-123-123/myRpcCall");
        }

    }
}
