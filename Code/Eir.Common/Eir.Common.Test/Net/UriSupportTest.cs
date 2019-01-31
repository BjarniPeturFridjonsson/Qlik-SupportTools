using System;
using Eir.Common.Net;
using NUnit.Framework;

namespace Eir.Common.Test.Net
{
    [TestFixture]
    public class UriSupportTest
    {
        [Test]
        [TestCase(null, null)]
        [TestCase("", null)]
        [TestCase(null, "")]
        [TestCase(null, "x")]
        public void Empty_baseUri_not_allowed(string baseUri, string apiEndpointPart)
        {
            Assert.Throws<ArgumentException>(() => UriSupport.Combine(baseUri, apiEndpointPart));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Empty_apiEndpointPart(string apiEndpointPart)
        {
            string uri = UriSupport.Combine("http://www.base.url", apiEndpointPart);
            Assert.That(uri, Is.EqualTo("http://www.base.url"));
        }

        [Test]
        public void No_glue_slash()
        {
            string uri = UriSupport.Combine("http://www.base.url", "part2");
            Assert.That(uri, Is.EqualTo("http://www.base.url/part2"));
        }

        [Test]
        public void Ending_glue_slash()
        {
            string uri = UriSupport.Combine("http://www.base.url/", "part2");
            Assert.That(uri, Is.EqualTo("http://www.base.url/part2"));
        }

        [Test]
        public void Beginning_glue_slash()
        {
            string uri = UriSupport.Combine("http://www.base.url", "/part2");
            Assert.That(uri, Is.EqualTo("http://www.base.url/part2"));
        }

        [Test]
        public void Double_glue_slash()
        {
            string uri = UriSupport.Combine("http://www.base.url/", "/part2");
            Assert.That(uri, Is.EqualTo("http://www.base.url/part2"));
        }

        [Test]
        public void One_extra_arg()
        {
            string uri = UriSupport.Combine("http://www.base.url", "part2", new UriArg("arg1", "val1"));
            Assert.That(uri, Is.EqualTo("http://www.base.url/part2?arg1=val1"));
        }

        [Test]
        public void One_extra_arg_when_ending_with_questionmark()
        {
            string uri = UriSupport.Combine("http://www.base.url", "part2?", new UriArg("arg1", "val1"));
            Assert.That(uri, Is.EqualTo("http://www.base.url/part2?arg1=val1"));
        }

        [Test]
        public void One_extra_arg_when_existing_arg()
        {
            string uri = UriSupport.Combine("http://www.base.url", "part2?arg0=val0", new UriArg("arg1", "val1"));
            Assert.That(uri, Is.EqualTo("http://www.base.url/part2?arg0=val0&arg1=val1"));
        }

        [Test]
        public void One_extra_arg_when_existing_arg_ending_with_ampersand()
        {
            string uri = UriSupport.Combine("http://www.base.url", "part2?arg0=val0&", new UriArg("arg1", "val1"));
            Assert.That(uri, Is.EqualTo("http://www.base.url/part2?arg0=val0&arg1=val1"));
        }

        [Test]
        public void Three_extra_args()
        {
            string uri = UriSupport.Combine("http://www.base.url", "part2", new UriArg("arg1", "val1"), new UriArg("arg2", "val2"), new UriArg("arg3", "val3"));
            Assert.That(uri, Is.EqualTo("http://www.base.url/part2?arg1=val1&arg2=val2&arg3=val3"));
        }

        [Test]
        public void One_extra_arg_with_illegal_chars()
        {
            string uri = UriSupport.Combine("http://www.base.url", "part2", new UriArg("arg:<?&>1", "val:<?&>1"));
            Assert.That(uri, Is.EqualTo("http://www.base.url/part2?arg%3A%3C%3F%26%3E1=val%3A%3C%3F%26%3E1"));
        }

        [Test]
        public void Three_extra_args_with_non_string_value()
        {
            string uri = UriSupport.Combine("http://www.base.url", "part2", new UriArg("arg1", 42), new UriArg("arg2", Guid.Empty), new UriArg("arg3", false));
            Assert.That(uri, Is.EqualTo("http://www.base.url/part2?arg1=42&arg2=00000000-0000-0000-0000-000000000000&arg3=False"));
        }

        [Test]
        public void One_extra_arg_with_null_value()
        {
            string uri = UriSupport.Combine("http://www.base.url", "part2", new UriArg("arg1", null));
            Assert.That(uri, Is.EqualTo("http://www.base.url/part2?arg1=null"));
        }
    }
}