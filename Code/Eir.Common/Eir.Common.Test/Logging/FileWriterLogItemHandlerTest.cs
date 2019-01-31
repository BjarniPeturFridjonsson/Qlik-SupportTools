using System;
using System.IO;
using Eir.Common.Common;
using Eir.Common.IO;
using Eir.Common.Logging;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Eir.Common.Test.Logging
{
    [TestFixture]
    public class FileWriterLogItemHandlerTest : TestBase
    {
        private readonly LogFileNameComposer _logFileNameComposer = new LogFileNameComposer("App", "Log", ".txt");

        private const string LOG_DIR = @"C:\path";
        private DateTime _currentTime;
        private IDateTimeProvider _dateTimeProvider;
        private FakeFileSystem _fileSystem;
        private FileWriterLogItemHandler<TestLogItem> _fileWriterLogItemHandler;
        private FileWriterLogItemHandler<TestLogItem2> _fileWriterLogItem2Handler;

        private const int ROLL_FILE_SIZE_THRESHOLD = 100; // Keep it low... :-)

        public class TestLogItem : LogItem
        {
            public static readonly TestLogItem Header = new TestLogItem();

            private TestLogItem()
                : base("Timestamp", "Message")
            {
            }

            public TestLogItem(DateTime timestamp, string message)
                : base(timestamp.ToString("O"), message)
            {
                Timestamp = timestamp;
                Message = message;
                LogLevel = LogLevel.None;
            }

            public override DateTime Timestamp { get; }

            public string Message { get; }

            public override LogLevel LogLevel { get; }
        }

        public class TestLogItem2 : LogItem
        {
            public static readonly TestLogItem2 Header = new TestLogItem2();

            private TestLogItem2()
                : base("Timestamp", "Message", "Extra")
            {
            }

            public TestLogItem2(DateTime timestamp, string message, string extra)
                : base(timestamp.ToString("O"), message, extra)
            {
                Timestamp = timestamp;
                Message = message;
                Extra = extra;
                LogLevel = LogLevel.None;
            }

            public override DateTime Timestamp { get; }

            public string Message { get; }
            public string Extra { get; set; }

            public override LogLevel LogLevel { get; }
        }

        [SetUp]
        public void SetUp()
        {
            _currentTime = new DateTime(2016, 1, 1, 12, 0, 0);

            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            dateTimeProviderMock.Setup(x => x.Time()).Returns(() => _currentTime);
            _dateTimeProvider = dateTimeProviderMock.Object;

            _fileSystem = new FakeFileSystem(_dateTimeProvider);

            _fileWriterLogItemHandler = new FileWriterLogItemHandler<TestLogItem>(
                LOG_DIR,
                _logFileNameComposer,
                _fileSystem,
                TestLogItem.Header,
                ROLL_FILE_SIZE_THRESHOLD);

            _fileWriterLogItem2Handler = new FileWriterLogItemHandler<TestLogItem2>(
                LOG_DIR,
                _logFileNameComposer,
                _fileSystem,
                TestLogItem2.Header,
                ROLL_FILE_SIZE_THRESHOLD);
        }

        [Test]
        public void Initialize_LogWriting_When_There_Is_A_Rolled_File_On_Disk()
        {
            var activeLogfilePath = Path.Combine(LOG_DIR, $"{_currentTime:yyyyMMdd}-App-Log.txt");
            var rolled1LogfilePath = Path.Combine(LOG_DIR, $"{_currentTime:yyyyMMdd}-App-Log.txt.1");
            var rolled2LogfilePath = Path.Combine(LOG_DIR, $"{_currentTime:yyyyMMdd}-App-Log.txt.2");

            var activeLogfileContent = new string('1', ROLL_FILE_SIZE_THRESHOLD);
            var rolledLogfileContent = new string('2', ROLL_FILE_SIZE_THRESHOLD);

            _fileSystem.SetFileFromUtf8String(activeLogfilePath, activeLogfileContent);
            _fileSystem.SetFileFromUtf8String(rolled1LogfilePath, rolledLogfileContent);


            _fileWriterLogItemHandler.Add(new TestLogItem(_currentTime, "Hello, Log!"));


            _fileSystem.GetFile(activeLogfilePath).ContentAsUtf8String.ShouldContain("Hello, Log!");
            _fileSystem.GetFile(rolled1LogfilePath).ContentAsUtf8String.ShouldBe(activeLogfileContent);
            _fileSystem.GetFile(rolled2LogfilePath).ContentAsUtf8String.ShouldBe(rolledLogfileContent);
        }

        [Test]
        public void Initialize_LogWriting_When_There_Is_A_Rolled_File_On_Disk_ThenRollOverToNewDay()
        {
            _currentTime = new DateTime(2016, 1, 1, 12, 0, 0);

            var day1ActiveLogfilePath = Path.Combine(LOG_DIR, $"{_currentTime:yyyyMMdd}-App-Log.txt");
            var day1Rolled1LogfilePath = Path.Combine(LOG_DIR, $"{_currentTime:yyyyMMdd}-App-Log.txt.1");
            var day1Rolled2LogfilePath = Path.Combine(LOG_DIR, $"{_currentTime:yyyyMMdd}-App-Log.txt.2");

            var day1ActiveLogfileContent = new string('A', ROLL_FILE_SIZE_THRESHOLD);
            var day1RolledLogfileContent = new string('B', ROLL_FILE_SIZE_THRESHOLD);

            _fileSystem.SetFileFromUtf8String(day1ActiveLogfilePath, day1ActiveLogfileContent);
            _fileSystem.SetFileFromUtf8String(day1Rolled1LogfilePath, day1RolledLogfileContent);

            _fileWriterLogItemHandler.Add(new TestLogItem(_currentTime, "Hello, Log - X!"));


            // so, now we have a situation where we have a couple of rolled files on 2016-01-01. Now lets push time
            // forward to a new day, and see write some more...
            _currentTime = _currentTime.AddDays(1);
            var day2ActiveLogfilePath = Path.Combine(LOG_DIR, $"{_currentTime:yyyyMMdd}-App-Log.txt");


            _fileWriterLogItemHandler.Add(new TestLogItem(_currentTime, "Hello, Log - Y!"));
            // ...and then force a log file roll
            _fileWriterLogItemHandler.Add(new TestLogItem(_currentTime, new string('C', ROLL_FILE_SIZE_THRESHOLD)));
            // this should perform the log roll for the second day


            // There was an issue related to resetting the rolled file count when we moved over midnight, which 
            // would retain the rolled log count from the previous day. In this test setup, we should not
            // have a file move on rolled file *.2 (to *.3) for the second day.
            _fileSystem.GetFile(day1ActiveLogfilePath).ContentAsUtf8String.ShouldContain("Hello, Log - X!");
            _fileSystem.GetFile(day1Rolled1LogfilePath).ContentAsUtf8String.ShouldBe(day1ActiveLogfileContent);
            _fileSystem.GetFile(day1Rolled2LogfilePath).ContentAsUtf8String.ShouldBe(day1RolledLogfileContent);
            _fileSystem.GetFile(day2ActiveLogfilePath).ContentAsUtf8String.ShouldContain("Hello, Log - Y!");
            _fileSystem.GetFile(day2ActiveLogfilePath).ContentAsUtf8String.ShouldContain("CCCCCCCCCCCCCC");
        }

        [Test]
        public void Initialize_LogWriting_Should_Roll_File_If_Headers_Have_Changed()
        {
            _currentTime = new DateTime(2016, 1, 1, 12, 0, 0);

            var day1ActiveLogfilePath = Path.Combine(LOG_DIR, $"{_currentTime:yyyyMMdd}-App-Log.txt");
            var day1Rolled1LogfilePath = Path.Combine(LOG_DIR, $"{_currentTime:yyyyMMdd}-App-Log.txt.1");

            _fileWriterLogItemHandler.Add(new TestLogItem(_currentTime, "Log item with two headers"));
            _fileWriterLogItemHandler.Dispose(); // make sure to release open stream writers
            _fileWriterLogItem2Handler.Add(new TestLogItem2(_currentTime, "Log item with three headers", "and extra info"));

            var activeFileContent = _fileSystem.GetFile(day1ActiveLogfilePath).ContentAsUtf8String;
            activeFileContent.ShouldContain("Log item with three headers");
            activeFileContent.ShouldContain("and extra info");
            activeFileContent.ShouldNotContain("Log item with two headers");

            var rolledFileContent = _fileSystem.GetFile(day1Rolled1LogfilePath).ContentAsUtf8String;
            rolledFileContent.ShouldContain("Log item with two headers");
            rolledFileContent.ShouldNotContain("Log item with three headers");
            rolledFileContent.ShouldNotContain("and extra info");
        }
    }
}