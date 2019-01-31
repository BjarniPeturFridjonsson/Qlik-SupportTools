using System;
using System.IO;
using Eir.Common.Common;
using Eir.Common.IO;
using Eir.Common.Test.Xml;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Eir.Common.Test.Common
{
    [TestFixture]
    public class SettingsInternalTest : TestBase
    {
        private const string ExpectedSettingsFile1 =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><root><node key=\"key\" value=\"value\" /></root>";

        [Test]
        public void Write()
        {
            var fileSystem = new FakeFileSystem(DateTimeProvider.Singleton);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml");
            fileSystem.SetFileFromUtf8String(path, ExpectedSettingsFile1);

            var settingsInternal = new SettingsInternal(fileSystem, DateTimeProvider.Singleton);
            settingsInternal.SetSetting("key", "value2");

            XmlTestSupport.GetComparableXml(fileSystem.GetFile(path).ContentAsUtf8String)
                .ShouldBe("<?xml version=\"1.0\" encoding=\"utf-8\"?><root><node key=\"key\" value=\"value2\" /></root>");
        }

        [Test]
        public void Read_SeveralTimes_VerifyReload()
        {
            var fileSystem = new FakeFileSystem(DateTimeProvider.Singleton);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml");
            IFakeFile settingsFile = fileSystem.SetFileFromUtf8String(path, ExpectedSettingsFile1);

            DateTime currentTime = new DateTime(2016, 1, 1, 12, 0, 0);

            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            dateTimeProviderMock
                .Setup(x => x.Time())
                .Returns(() => currentTime);

            var settingsInternal = new SettingsInternal(fileSystem, dateTimeProviderMock.Object);

            var value = settingsInternal.GetSetting("key"); // should cause reload

            currentTime = currentTime.AddSeconds(32);
            value = settingsInternal.GetSetting("key");

            currentTime = currentTime.AddSeconds(32); // should cause reload
            value = settingsInternal.GetSetting("key");

            FileAccessInfo fileAccessInfo = fileSystem.GetFileAccessInfo(path);
            Assert.That(fileAccessInfo.Reads, Is.EqualTo(2));
            Assert.That(fileAccessInfo.Writes, Is.EqualTo(0));
        }
    }
}