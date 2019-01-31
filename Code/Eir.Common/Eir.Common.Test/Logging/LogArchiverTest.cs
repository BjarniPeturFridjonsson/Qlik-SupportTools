using System;
using Eir.Common.Common;
using Eir.Common.IO;
using Eir.Common.Logging;
using Eir.Common.Time;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Eir.Common.Test.Logging
{
    [TestFixture]
    public class LogArchiverTest : TestBase
    {
        private const string LOG_DIR = @"C:\path\";
        private const int DAYS_TO_IGNORE = 3;

        private static readonly ILogFileNameComposer _logFileNameComposer = new LogFileNameComposer("MyApp", "MyLog", ".MyExt");
        private IDateTimeProvider _dateTimeProvider;
        private DateTime _now;
        private FakeFileSystem _fileSystem;
        private DirectTrigger _trigger;
        private LogArchiver _logArchiver;

        [SetUp]
        public void SetUp()
        {
            _now = new DateTime(2008, 09, 10, 3, 4, 5);

            var mockDateTimeProvider = new Mock<IDateTimeProvider>();
            mockDateTimeProvider.Setup(x => x.Time()).Returns(() => _now);
            _dateTimeProvider = mockDateTimeProvider.Object;

            _fileSystem = new FakeFileSystem(_dateTimeProvider, true);

            _trigger = new DirectTrigger();

            _logArchiver = new LogArchiver(
                LOG_DIR,
                DAYS_TO_IGNORE,
                _fileSystem,
                _logFileNameComposer,
                _dateTimeProvider,
                _trigger);
        }

        [TearDown]
        public void TearDown()
        {
            _fileSystem.PrintToConsole();
            _logArchiver.Dispose();
        }

        [Test]
        public void Smoketest()
        {
            _trigger.Trig();
        }

        private IFakeFile CreateLogFile(DateTime date, string content, string rollNumber = "")
        {
            DateTime now = _now;
            _now = date; // By setting the "_now" the virtual file will have the correct time stamp.
            var file = _fileSystem.SetFileFromUtf8String(LOG_DIR + $"{date:yyyyMMdd}-MyApp-MyLog.MyExt{rollNumber}", content);
            _now = now;
            return file;
        }

        private IFakeFile CreateArchiveFile(DateTime date, string base64Content)
        {
            DateTime now = _now;
            _now = date; // By setting the "_now" the virtual file will have the correct time stamp.
            var file = _fileSystem.SetFileFromBase64(LOG_DIR + $"Archived_{date:yyyyMMdd}.zip", base64Content);
            _now = now;
            return file;
        }

        [Test]
        public void Only_new_files_exists()
        {
            CreateLogFile(new DateTime(2008, 9, 10), "The content 1");
            CreateLogFile(new DateTime(2008, 9, 9), "The content 2");
            CreateLogFile(new DateTime(2008, 9, 8), "The content 3");

            _trigger.Trig();

            _fileSystem.AssertFileList(
                LOG_DIR + "20080908-MyApp-MyLog.MyExt",
                LOG_DIR + "20080909-MyApp-MyLog.MyExt",
                LOG_DIR + "20080910-MyApp-MyLog.MyExt");
        }

        [Test]
        public void One_old_file_exist()
        {
            CreateLogFile(new DateTime(2008, 9, 10), "The content 1");
            CreateLogFile(new DateTime(2008, 9, 9), "The content 2");
            CreateLogFile(new DateTime(2008, 9, 8), "The content 3");
            CreateLogFile(new DateTime(2008, 9, 7), "The content 4");

            _trigger.Trig();

            _fileSystem.AssertFileList(
                LOG_DIR + "20080908-MyApp-MyLog.MyExt",
                LOG_DIR + "20080909-MyApp-MyLog.MyExt",
                LOG_DIR + "20080910-MyApp-MyLog.MyExt",
                LOG_DIR + "Archived_20080907.zip");

            _fileSystem.GetFile(LOG_DIR + "Archived_20080907.zip").ContentAsBase64
                .ShouldBe("UEsDBBQAAAAIAAAAJzkcxUfmEgAAABAAAAAaAAAAMjAwODA5MDctTXlBcHAtTXlMb2cuTXlFeHR7v3" +
                          "t/SEaqQnJ+XklqXomCCQBQSwECMwAUAAAACAAAACc5HMVH5hIAAAAQAAAAGgAAAAAAAAAAAAAAAA" +
                          "AAAAAAMjAwODA5MDctTXlBcHAtTXlMb2cuTXlFeHRQSwUGAAAAAAEAAQBIAAAASgAAAAAA");
        }

        [Test]
        public void Some_old_files_exists()
        {
            CreateLogFile(new DateTime(2008, 9, 10), "The content 1");
            CreateLogFile(new DateTime(2008, 9, 9), "The content 2");
            CreateLogFile(new DateTime(2008, 9, 8), "The content 3");
            CreateLogFile(new DateTime(2008, 9, 7), "The content 4");
            CreateLogFile(new DateTime(2008, 9, 7), "The content 5", ".1");
            CreateLogFile(new DateTime(2008, 9, 7), "The content 6", ".2");
            CreateLogFile(new DateTime(2008, 9, 6), "The content 7");
            CreateLogFile(new DateTime(2008, 9, 5), "The content 8");
            CreateLogFile(new DateTime(2008, 9, 5), "The content 9", ".1");
            CreateLogFile(new DateTime(2008, 9, 5), "The content 10", ".2");
            CreateLogFile(new DateTime(2008, 9, 5), "The content 11", ".3");

            _trigger.Trig();

            _fileSystem.AssertFileList(
                LOG_DIR + "20080908-MyApp-MyLog.MyExt",
                LOG_DIR + "20080909-MyApp-MyLog.MyExt",
                LOG_DIR + "20080910-MyApp-MyLog.MyExt",
                LOG_DIR + "Archived_20080907.zip",
                LOG_DIR + "Archived_20080906.zip",
                LOG_DIR + "Archived_20080905.zip");

            _fileSystem.GetFile(LOG_DIR + "Archived_20080907.zip").ContentAsBase64
                .ShouldBe("UEsDBBQAAAAIAAAAJzkcxUfmEgAAABAAAAAaAAAAMjAwODA5MDctTXlBcHAtTXlMb2cuTXlFeHR7v3t/SEa" +
                          "qQnJ+XklqXomCCQBQSwMEFAAAAAgAAAAnOYr1QJESAAAAEAAAABwAAAAyMDA4MDkwNy1NeUFwcC1NeUxvZy" +
                          "5NeUV4dC4xe797f0hGqkJyfl5Jal6JgikAUEsDBBQAAAAIAAAAJzkwpEkIEgAAABAAAAAcAAAAMjAwODA5M" +
                          "DctTXlBcHAtTXlMb2cuTXlFeHQuMnu/e39IRqpCcn5eSWpeiYIZAFBLAQIzABQAAAAIAAAAJzkcxUfmEgAA" +
                          "ABAAAAAaAAAAAAAAAAAAAAAAAAAAAAAyMDA4MDkwNy1NeUFwcC1NeUxvZy5NeUV4dFBLAQIzABQAAAAIAAA" +
                          "AJzmK9UCREgAAABAAAAAcAAAAAAAAAAAAAAAAAEoAAAAyMDA4MDkwNy1NeUFwcC1NeUxvZy5NeUV4dC4xUE" +
                          "sBAjMAFAAAAAgAAAAnOTCkSQgSAAAAEAAAABwAAAAAAAAAAAAAAAAAlgAAADIwMDgwOTA3LU15QXBwLU15T" +
                          "G9nLk15RXh0LjJQSwUGAAAAAAMAAwDcAAAA4gAAAAAA");

            _fileSystem.GetFile(LOG_DIR + "Archived_20080906.zip").ContentAsBase64
                .ShouldBe("UEsDBBQAAAAIAAAAJjmmlE5/EgAAABAAAAAaAAAAMjAwODA5MDYtTXlBcHAtTXlMb2cuTXlFeHR7v3t/S" +
                          "EaqQnJ+XklqXomCOQBQSwECMwAUAAAACAAAACY5ppROfxIAAAAQAAAAGgAAAAAAAAAAAAAAAAAAAAAAMjAw" +
                          "ODA5MDYtTXlBcHAtTXlMb2cuTXlFeHRQSwUGAAAAAAEAAQBIAAAASgAAAAAA");

            _fileSystem.GetFile(LOG_DIR + "Archived_20080905.zip").ContentAsBase64
                .ShouldBe("UEsDBBQAAAAIAAAAJTk3ifHvEgAAABAAAAAaAAAAMjAwODA5MDUtTXlBcHAtTXlMb2cuTXlFeHR7v3t/SEaq" +
                          "QnJ+XklqXomCBQBQSwMEFAAAAAgAAAAlOaG59pgSAAAAEAAAABwAAAAyMDA4MDkwNS1NeUFwcC1NeUxvZy" +
                          "5NeUV4dC4xe797f0hGqkJyfl5Jal6JgiUAUEsDBBQAAAAIAAAAJTnuMEudEwAAABEAAAAcAAAAMjAwODA5MD" +
                          "UtTXlBcHAtTXlMb2cuTXlFeHQuMnu/e39IRqpCcn5eSWpeiYKhAQBQSwMEFAAAAAgAAAAlOXgATOoTAAAAEQ" +
                          "AAABwAAAAyMDA4MDkwNS1NeUFwcC1NeUxvZy5NeUV4dC4ze797f0hGqkJyfl5Jal6JgqEhAFBLAQIzABQAAA" +
                          "AIAAAAJTk3ifHvEgAAABAAAAAaAAAAAAAAAAAAAAAAAAAAAAAyMDA4MDkwNS1NeUFwcC1NeUxvZy5NeUV4dF" +
                          "BLAQIzABQAAAAIAAAAJTmhufaYEgAAABAAAAAcAAAAAAAAAAAAAAAAAEoAAAAyMDA4MDkwNS1NeUFwcC1NeU" +
                          "xvZy5NeUV4dC4xUEsBAjMAFAAAAAgAAAAlOe4wS50TAAAAEQAAABwAAAAAAAAAAAAAAAAAlgAAADIwMDgwOT" +
                          "A1LU15QXBwLU15TG9nLk15RXh0LjJQSwECMwAUAAAACAAAACU5eABM6hMAAAARAAAAHAAAAAAAAAAAAAAAAA" +
                          "DjAAAAMjAwODA5MDUtTXlBcHAtTXlMb2cuTXlFeHQuM1BLBQYAAAAABAAEACYBAAAwAQAAAAA=");
        }

        [Test]
        public void Some_old_file_is_locked()
        {
            CreateLogFile(new DateTime(2008, 9, 5), "The content 1");
            CreateLogFile(new DateTime(2008, 9, 6), "The content 2").Locked = true;
            CreateLogFile(new DateTime(2008, 9, 7), "The content 3");

            _trigger.Trig();

            _fileSystem.AssertFileList(
                LOG_DIR + "Archived_20080905.zip",
                LOG_DIR + "20080906-MyApp-MyLog.MyExt",
                LOG_DIR + "Archived_20080907.zip");

            _fileSystem.GetFile(LOG_DIR + "Archived_20080905.zip").ContentAsBase64
                .ShouldBe("UEsDBBQAAAAIAAAAJTmTMS2WEgAAABAAAAAaAAAAMjAwODA5MDUtTXlBcHAtTXlMb2cuTXlFeHR" +
                          "7v3t/SEaqQnJ+XklqXomCIQBQSwECMwAUAAAACAAAACU5kzEtlhIAAAAQAAAAGgAAAAAAAAAAAA" +
                          "AAAAAAAAAAMjAwODA5MDUtTXlBcHAtTXlMb2cuTXlFeHRQSwUGAAAAAAEAAQBIAAAASgAAAAAA");

            _fileSystem.GetFile(LOG_DIR + "Archived_20080907.zip").ContentAsBase64
                .ShouldBe("UEsDBBQAAAAIAAAAJzm/UCN4EgAAABAAAAAaAAAAMjAwODA5MDctTXlBcHAtTXlMb2cuTXlFeHR7" +
                          "v3t/SEaqQnJ+XklqXomCMQBQSwECMwAUAAAACAAAACc5v1AjeBIAAAAQAAAAGgAAAAAAAAAAAAA" +
                          "AAAAAAAAAMjAwODA5MDctTXlBcHAtTXlMb2cuTXlFeHRQSwUGAAAAAAEAAQBIAAAASgAAAAAA");
        }

        [Test]
        public void New_log_file_are_added_to_existing_zip()
        {
            // This file...
            // CreateLogFile(new DateTime(2008, 9, 3), "The content A");
            // ...is now in this zip-file:
            CreateArchiveFile(new DateTime(2008, 9, 3), "UEsDBBQAAAAIAAAAIzmvQCjGEgAAABAAAAAaAAAAMjAwODA5MDMtTXlBcHAtTXlMb2c" +
                                                        "uTXlFeHR7v3t/SEaqQnJ+XklqXomCIwBQSwECMwAUAAAACAAAACM5r0AoxhIAAAAQAAA" +
                                                        "AGgAAAAAAAAAAAAAAAAAAAAAAMjAwODA5MDMtTXlBcHAtTXlMb2cuTXlFeHRQSwUGAAAAAAEAAQBIAAAASgAAAAAA");
            // ...but then /this/ file suddenly appeared
            CreateLogFile(new DateTime(2008, 9, 3), "The content B", ".1");

            _trigger.Trig();

            _fileSystem.AssertFileList(
                LOG_DIR + "Archived_20080903.zip");

            _fileSystem.GetFile(LOG_DIR + "Archived_20080903.zip").ContentAsBase64
                .ShouldBe("UEsDBBQAAAAIAAAAIzmvQCjGEgAAABAAAAAaAAAAMjAwODA5MDMtTXlBcHAtTXlMb2cuTXlF" +
                          "eHR7v3t/SEaqQnJ+XklqXomCIwBQSwMEFAAAAAgAAAAjORURIV8SAAAAEAAAABwAAAAyMDA4" +
                          "MDkwMy1NeUFwcC1NeUxvZy5NeUV4dC4xe797f0hGqkJyfl5Jal6JghMAUEsBAjMAFAAAAAgAA" +
                          "AAjOa9AKMYSAAAAEAAAABoAAAAAAAAAAAAAAAAAAAAAADIwMDgwOTAzLU15QXBwLU15TG9nL" +
                          "k15RXh0UEsBAjMAFAAAAAgAAAAjORURIV8SAAAAEAAAABwAAAAAAAAAAAAAAAAASgAAADIwMD" +
                          "gwOTAzLU15QXBwLU15TG9nLk15RXh0LjFQSwUGAAAAAAIAAgCSAAAAlgAAAAAA");
        }

        [Test]
        public void Unchanged_log_file_is_not_added_to_existing_zip___just_deleted()
        {
            // This file...
            // CreateLogFile(new DateTime(2008, 9, 3), "The content A");
            // ...is now in this zip-file:
            CreateArchiveFile(new DateTime(2008, 9, 3), "UEsDBBQAAAAIAAAAIzmvQCjGEgAAABAAAAAaAAAAMjAwODA5MDMtTXlBcHAtTXlMb2cuTXlF" +
                          "eHR7v3t/SEaqQnJ+XklqXomCIwBQSwMEFAAAAAgAAAAjORURIV8SAAAAEAAAABwAAAAyMDA4" +
                          "MDkwMy1NeUFwcC1NeUxvZy5NeUV4dC4xe797f0hGqkJyfl5Jal6JghMAUEsBAjMAFAAAAAgAA" +
                          "AAjOa9AKMYSAAAAEAAAABoAAAAAAAAAAAAAAAAAAAAAADIwMDgwOTAzLU15QXBwLU15TG9nL" +
                          "k15RXh0UEsBAjMAFAAAAAgAAAAjORURIV8SAAAAEAAAABwAAAAAAAAAAAAAAAAASgAAADIwMD" +
                          "gwOTAzLU15QXBwLU15TG9nLk15RXh0LjFQSwUGAAAAAAIAAgCSAAAAlgAAAAAA");

            // ...but then /this/ file suddenly appeared
            CreateLogFile(new DateTime(2008, 9, 3), "The content B", ".1");

            _trigger.Trig();

            _fileSystem.AssertFileList(
                LOG_DIR + "Archived_20080903.zip");

            _fileSystem.GetFileAccessInfo(LOG_DIR + "Archived_20080903.zip").Writes.ShouldBe(0);

            _fileSystem.GetFile(LOG_DIR + "Archived_20080903.zip").ContentAsBase64
                .ShouldBe("UEsDBBQAAAAIAAAAIzmvQCjGEgAAABAAAAAaAAAAMjAwODA5MDMtTXlBcHAtTXlMb2cuTXlF" +
                          "eHR7v3t/SEaqQnJ+XklqXomCIwBQSwMEFAAAAAgAAAAjORURIV8SAAAAEAAAABwAAAAyMDA4" +
                          "MDkwMy1NeUFwcC1NeUxvZy5NeUV4dC4xe797f0hGqkJyfl5Jal6JghMAUEsBAjMAFAAAAAgAA" +
                          "AAjOa9AKMYSAAAAEAAAABoAAAAAAAAAAAAAAAAAAAAAADIwMDgwOTAzLU15QXBwLU15TG9nL" +
                          "k15RXh0UEsBAjMAFAAAAAgAAAAjORURIV8SAAAAEAAAABwAAAAAAAAAAAAAAAAASgAAADIwMD" +
                          "gwOTAzLU15QXBwLU15TG9nLk15RXh0LjFQSwUGAAAAAAIAAgCSAAAAlgAAAAAA");
        }

        [Test]
        public void Changed_log_file_is_added_to_existing_zip___new_date()
        {
            // This file...
            // CreateLogFile(new DateTime(2008, 9, 3), "The content A");
            // ...is now in this zip-file:
            CreateArchiveFile(new DateTime(2008, 9, 3), "UEsDBBQAAAAIAAAAIzmvQCjGEgAAABAAAAAaAAAAMjAwODA5MDMtTXlBcHAtTXlMb2cuTXlF" +
                          "eHR7v3t/SEaqQnJ+XklqXomCIwBQSwMEFAAAAAgAAAAjORURIV8SAAAAEAAAABwAAAAyMDA4" +
                          "MDkwMy1NeUFwcC1NeUxvZy5NeUV4dC4xe797f0hGqkJyfl5Jal6JghMAUEsBAjMAFAAAAAgAA" +
                          "AAjOa9AKMYSAAAAEAAAABoAAAAAAAAAAAAAAAAAAAAAADIwMDgwOTAzLU15QXBwLU15TG9nL" +
                          "k15RXh0UEsBAjMAFAAAAAgAAAAjORURIV8SAAAAEAAAABwAAAAAAAAAAAAAAAAASgAAADIwMD" +
                          "gwOTAzLU15QXBwLU15TG9nLk15RXh0LjFQSwUGAAAAAAIAAgCSAAAAlgAAAAAA");

            // ...but then /this/ file suddenly appeared
            CreateLogFile(new DateTime(2008, 9, 3), "The content B", ".1").LastWriteTimeUtc = new DateTime(2008, 9, 4); // Same content as in the zip file, but a newer write time.

            _trigger.Trig();

            _fileSystem.AssertFileList(
                LOG_DIR + "Archived_20080903.zip");

            _fileSystem.GetFileAccessInfo(LOG_DIR + "Archived_20080903.zip").Writes.ShouldBe(1); // Written once!

            _fileSystem.GetFile(LOG_DIR + "Archived_20080903.zip").ContentAsBase64
                .ShouldBe("UEsDBBQAAAAIAAAAIzmvQCjGEgAAABAAAAAaAAAAMjAwODA5MDMtTXlBcHAtTXlMb2cuTX" +
                          "lFeHR7v3t/SEaqQnJ+XklqXomCIwBQSwMEFAAAAAgAAAAkORURIV8SAAAAEAAAABwAAAAy" +
                          "MDA4MDkwMy1NeUFwcC1NeUxvZy5NeUV4dC4xe797f0hGqkJyfl5Jal6JghMAUEsBAjMAFA" +
                          "AAAAgAAAAjOa9AKMYSAAAAEAAAABoAAAAAAAAAAAAAAAAAAAAAADIwMDgwOTAzLU15QXBw" +
                          "LU15TG9nLk15RXh0UEsBAjMAFAAAAAgAAAAkORURIV8SAAAAEAAAABwAAAAAAAAAAAAAAAA" +
                          "ASgAAADIwMDgwOTAzLU15QXBwLU15TG9nLk15RXh0LjFQSwUGAAAAAAIAAgCSAAAAlgAAAAAA");
        }

        [Test]
        public void Changed_log_file_is_added_to_existing_zip___new_content()
        {
            // This file...
            // CreateLogFile(new DateTime(2008, 9, 3), "The content A");
            // ...is now in this zip-file:
            CreateArchiveFile(new DateTime(2008, 9, 3), "UEsDBBQAAAAIAAAAIzmvQCjGEgAAABAAAAAaAAAAMjAwODA5MDMtTXlBcHAtTXlMb2cuTXlF" +
                          "eHR7v3t/SEaqQnJ+XklqXomCIwBQSwMEFAAAAAgAAAAjORURIV8SAAAAEAAAABwAAAAyMDA4" +
                          "MDkwMy1NeUFwcC1NeUxvZy5NeUV4dC4xe797f0hGqkJyfl5Jal6JghMAUEsBAjMAFAAAAAgAA" +
                          "AAjOa9AKMYSAAAAEAAAABoAAAAAAAAAAAAAAAAAAAAAADIwMDgwOTAzLU15QXBwLU15TG9nL" +
                          "k15RXh0UEsBAjMAFAAAAAgAAAAjORURIV8SAAAAEAAAABwAAAAAAAAAAAAAAAAASgAAADIwMD" +
                          "gwOTAzLU15QXBwLU15TG9nLk15RXh0LjFQSwUGAAAAAAIAAgCSAAAAlgAAAAAA");

            // ...but then /this/ file suddenly appeared
            CreateLogFile(new DateTime(2008, 9, 3), "The content B2", ".1"); // Different content length than in the zip file.

            _trigger.Trig();

            _fileSystem.AssertFileList(
                LOG_DIR + "Archived_20080903.zip");

            _fileSystem.GetFileAccessInfo(LOG_DIR + "Archived_20080903.zip").Writes.ShouldBe(1); // Written once!

            _fileSystem.GetFile(LOG_DIR + "Archived_20080903.zip").ContentAsBase64
                .ShouldBe("UEsDBBQAAAAIAAAAIzmvQCjGEgAAABAAAAAaAAAAMjAwODA5MDMtTXlBcHAtTXlMb2cuTXlF" +
                          "eHR7v3t/SEaqQnJ+XklqXomCIwBQSwMEFAAAAAgAAAAjOfd7V3cTAAAAEQAAABwAAAAyMDA4" +
                          "MDkwMy1NeUFwcC1NeUxvZy5NeUV4dC4xe797f0hGqkJyfl5Jal6JgpMRAFBLAQIzABQAAAAIA" +
                          "AAAIzmvQCjGEgAAABAAAAAaAAAAAAAAAAAAAAAAAAAAAAAyMDA4MDkwMy1NeUFwcC1NeUxvZy5N" +
                          "eUV4dFBLAQIzABQAAAAIAAAAIzn3e1d3EwAAABEAAAAcAAAAAAAAAAAAAAAAAEoAAAAyMDA4M" +
                          "DkwMy1NeUFwcC1NeUxvZy5NeUV4dC4xUEsFBgAAAAACAAIAkgAAAJcAAAAAAA==");
        }
    }
}