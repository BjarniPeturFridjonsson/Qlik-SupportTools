using System.Globalization;
using Eir.Common.Extensions;
using NUnit.Framework;
using Shouldly;

namespace Eir.Common.Test.Extensions
{
    [TestFixture]
    public class Int64ExtensionsTest
    {
        [Test]
        public void Value1023_GivesB()
        {
            long size = 1023;
            size.AsByteSizeString().ShouldBe("1023 B");
        }

        [Test]
        public void Value1048000_GivesKB()
        {
            long size = 1048000;
            size.AsByteSizeString().ShouldBe("1023.4 KB");
        }

        [Test]
        public void Value1073152000_GivesMB()
        {
            long size = 1073152000;
            size.AsByteSizeString().ShouldBe("1023.4 MB");
        }

        [Test]
        public void UsesDecimalSeparatorFromGivenFormatProvider()
        {
            long size = 1073152000;
            size.AsByteSizeString(CultureInfo.GetCultureInfo("en-US")).ShouldBe("1023.4 MB");
            size.AsByteSizeString(CultureInfo.GetCultureInfo("sv-SE")).ShouldBe("1023,4 MB");
        }

        [Test]
        public void Value1098907648000_GivesGB()
        {
            long size = 1098907648000;
            size.AsByteSizeString().ShouldBe("1023.4 GB");
        }

        [Test]
        public void Value1125281431552000_GivesGB()
        {
            long size = 1125281499552100;
            size.AsByteSizeString().ShouldBe("1048000.1 GB");
        }
    }
}