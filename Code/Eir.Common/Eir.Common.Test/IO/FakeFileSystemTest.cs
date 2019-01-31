using System.Linq;
using Eir.Common.Common;
using Eir.Common.IO;
using NUnit.Framework;
using Shouldly;

namespace Eir.Common.Test.IO
{
    [TestFixture]
    public class FakeFileSystemTest : TestBase
    {
        [Test]
        public void EnumerateDirectories_Empty()
        {
            var ffs = new FakeFileSystem(DateTimeProvider.Singleton);
            IFileSystem fs = ffs;

            var dirs = fs.EnumerateDirectories(@"C:\AAA");

            dirs.ToArray().ShouldBe(new string[0]);
        }

        [Test]
        public void EnumerateDirectories_One_ensured_dir()
        {
            var ffs = new FakeFileSystem(DateTimeProvider.Singleton);
            IFileSystem fs = ffs;
            fs.EnsureDirectory(@"C:\AAA\BBB");

            var dirs = fs.EnumerateDirectories(@"C:\AAA");

            dirs.ToArray().ShouldBe(new[] { @"C:\AAA\BBB" });
        }

        [Test]
        public void EnumerateDirectories_One_ensured_sub_dir()
        {
            var ffs = new FakeFileSystem(DateTimeProvider.Singleton);
            IFileSystem fs = ffs;
            fs.EnsureDirectory(@"C:\AAA\BBB\CCC");
            fs.EnsureDirectory(@"C:\AAA\BBB\CCC\DDD");

            var dirs = fs.EnumerateDirectories(@"C:\AAA");

            dirs.ToArray().ShouldBe(new[] { @"C:\AAA\BBB" });
        }

        [Test]
        public void EnumerateDirectories_One_file_in_sub_dir()
        {
            var ffs = new FakeFileSystem(DateTimeProvider.Singleton);
            IFileSystem fs = ffs;
            ffs.SetFileFromUtf8String(@"C:\AAA\BBB\a1.txt", "");

            var dirs = fs.EnumerateDirectories(@"C:\AAA");

            dirs.ToArray().ShouldBe(new[] { @"C:\AAA\BBB" });
        }

        [Test]
        public void EnumerateDirectories_One_file_in_dir()
        {
            var ffs = new FakeFileSystem(DateTimeProvider.Singleton);
            IFileSystem fs = ffs;
            ffs.SetFileFromUtf8String(@"C:\AAA\a1.txt", "");

            var dirs = fs.EnumerateDirectories(@"C:\AAA");

            dirs.ToArray().ShouldBe(new string[0]);
        }

        [Test]
        public void EnumerateDirectories_One_file_in_sub_sub_dir()
        {
            var ffs = new FakeFileSystem(DateTimeProvider.Singleton);
            IFileSystem fs = ffs;
            ffs.SetFileFromUtf8String(@"C:\AAA\BBB\CCC\a1.txt", "");

            var dirs = fs.EnumerateDirectories(@"C:\AAA");

            dirs.ToArray().ShouldBe(new[] { @"C:\AAA\BBB" });
        }

        [Test]
        public void EnumerateDirectories_Two_files_in_sub_sub_dirs()
        {
            var ffs = new FakeFileSystem(DateTimeProvider.Singleton);
            IFileSystem fs = ffs;
            ffs.SetFileFromUtf8String(@"C:\AAA\BBB\CCC\a1.txt", "");
            ffs.SetFileFromUtf8String(@"C:\AAA\BBB\DDD\a2.txt", "");

            var dirs = fs.EnumerateDirectories(@"C:\AAA");

            dirs.ToArray().ShouldBe(new[] { @"C:\AAA\BBB" });
        }

        [Test]
        public void EnumerateDirectories_Multiple_files_in_multiple_sub_sub_dirs()
        {
            var ffs = new FakeFileSystem(DateTimeProvider.Singleton);
            IFileSystem fs = ffs;
            ffs.SetFileFromUtf8String(@"C:\AAA\BBB\CCC\a1.txt", "");
            ffs.SetFileFromUtf8String(@"C:\AAA\BBB\DDD\a2.txt", "");
            ffs.SetFileFromUtf8String(@"C:\AAA\CCC\c1.txt", "");
            ffs.SetFileFromUtf8String(@"C:\AAA\DDD\d1.txt", "");
            ffs.SetFileFromUtf8String(@"C:\BBB\XXX\x1.txt", "");

            var dirs = fs.EnumerateDirectories(@"C:\AAA");

            dirs.ToArray().ShouldBe(new[]
            {
                @"C:\AAA\BBB",
                @"C:\AAA\CCC",
                @"C:\AAA\DDD",
            });
        }

        [Test]
        public void EnumerateDirectories_One_file_in_sub_dir_and_same_ensured_dir()
        {
            var ffs = new FakeFileSystem(DateTimeProvider.Singleton);
            IFileSystem fs = ffs;
            ffs.SetFileFromUtf8String(@"C:\AAA\BBB\a1.txt", "");
            fs.EnsureDirectory(@"C:\AAA\BBB");

            var dirs = fs.EnumerateDirectories(@"C:\AAA");

            dirs.ToArray().ShouldBe(new[] { @"C:\AAA\BBB" });
        }
    }
}