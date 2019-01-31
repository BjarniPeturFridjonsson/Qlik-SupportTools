using System;
using Eir.Common.Logging;
using NUnit.Framework;
using Shouldly;

namespace Eir.Common.Test.Logging
{
    [TestFixture]
    public class LogFileNameComposerTest : TestBase
    {
        [Test]
        public void Prefix_works()
        {
            ILogFileNameComposer logFileNameComposer = new LogFileNameComposer("MyApp", "MyLog", ".MyExt");

            logFileNameComposer.GetDatePart(new DateTime(2001, 2, 3)).ShouldBe("20010203");
        }

        [Test]
        public void GetFileName_works()
        {
            ILogFileNameComposer logFileNameComposer = new LogFileNameComposer("MyApp", "MyLog", ".MyExt");

            logFileNameComposer.GetFileName("20010203").ShouldBe("20010203-MyApp-MyLog.MyExt");
        }

        [Test]
        public void GetFileName_with_roll_works()
        {
            ILogFileNameComposer logFileNameComposer = new LogFileNameComposer("MyApp", "MyLog", ".MyExt");

            logFileNameComposer.GetFileName("20010203", "1").ShouldBe("20010203-MyApp-MyLog.MyExt.1");
        }

        [Test]
        public void OK_TrySplit_works()
        {
            ILogFileNameComposer logFileNameComposer = new LogFileNameComposer("MyApp", "MyLog", ".MyExt");

            string prefix, roll;

            logFileNameComposer.TryParse("20010203-MyApp-MyLog.MyExt", out prefix, out roll).ShouldBe(true);

            prefix.ShouldBe("20010203");
            roll.ShouldBe(null);
        }

        [Test]
        public void Bad_TrySplit_works()
        {
            ILogFileNameComposer logFileNameComposer = new LogFileNameComposer("MyApp", "MyLog", ".MyExt");

            string prefix, roll;

            logFileNameComposer.TryParse("20010203-BAD-MyApp-MyLog.MyExt", out prefix, out roll).ShouldBe(false);
        }

        [Test]
        public void OK_TrySplit_with_roll_works()
        {
            ILogFileNameComposer logFileNameComposer = new LogFileNameComposer("MyApp", "MyLog", ".MyExt");

            string prefix, roll;

            logFileNameComposer.TryParse("20010203-MyApp-MyLog.MyExt.1", out prefix, out roll).ShouldBe(true);

            prefix.ShouldBe("20010203");
            roll.ShouldBe("1");
        }
    }
}