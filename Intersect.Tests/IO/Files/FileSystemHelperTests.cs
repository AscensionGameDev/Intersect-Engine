using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;

using NUnit.Framework;

namespace Intersect.IO.Files
{
    [TestFixture]
    public class FileSystemHelperTests
    {
        [OneTimeSetUp]
        public void BeforeAll()
        {
            FileSystemHelper.FileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    {@"/directory/that/exists", new MockDirectoryData()},
                    {@"/directory/that/exists/with/a/file/that/exists", new MockFileData("")},
                }
            );
        }

        [TestCase("0B", 0)]
        [TestCase("1B", 1)]
        [TestCase("-1B", -1)]
        [TestCase("1KB", 1000)]
        [TestCase("-1KB", -1000)]
        [TestCase("1.024KB", 1024)]
        [TestCase("-1.024KB", -1024)]
        [TestCase("1000B", 1000, true)]
        [TestCase("-1000B", -1000, true)]
        [TestCase("1KiB", 1024, true)]
        [TestCase("-1KiB", -1024, true)]
        [TestCase("1MB", 1000000)]
        [TestCase("-1MB", -1000000)]
        [TestCase("1GB", 1000000000)]
        [TestCase("-1GB", -1000000000)]
        [TestCase("1TB", 1000000000000)]
        [TestCase("-1TB", -1000000000000)]
        [TestCase("1PB", 1000000000000000)]
        [TestCase("-1PB", -1000000000000000)]
        [TestCase("1EB", 1000000000000000000)]
        [TestCase("-1EB", -1000000000000000000)]
        [TestCase("1EB", 1000000000000000001)]
        [TestCase("-1EB", -1000000000000000001)]
        [TestCase("1.235EB", 1234567890123456789)]
        [TestCase("-1.235EB", -1234567890123456789)]
        public void TestFormatSize(string expected, long bytes, bool binary = false)
        {
            Assert.AreEqual(expected, FileSystemHelper.FormatSize(bytes, binary));
        }

        private static IEnumerable<string[]> RelativePathTestCases()
        {
            #region Unix-style paths

            yield return new[]
            {
                @"/directory/that/exists", @"/directory/that/does/not/exist",
                Path.Combine("..", "does", "not", "exist")
            };

            yield return new[]
            {
                @"/directory/that/exists", @"/directory/that/does/not/exist/",
                Path.Combine("..", "does", "not", "exist") + Path.DirectorySeparatorChar
            };

            yield return new[]
            {
                @"/directory/that/exists/", @"/directory/that/does/not/exist",
                Path.Combine("..", "does", "not", "exist")
            };

            yield return new[]
            {
                @"/directory/that/exists/", @"/directory/that/does/not/exist/",
                Path.Combine("..", "does", "not", "exist") + Path.DirectorySeparatorChar
            };

            yield return new[]
            {
                @"/directory/that/exists", @"/directory/that/exists/with/a/file/that/exists",
                Path.Combine("with", "a", "file", "that", "exists")
            };

            yield return new[]
            {
                @"/directory/that/exists/", @"/directory/that/exists/with/a/file/that/exists",
                Path.Combine("with", "a", "file", "that", "exists")
            };

            yield return new[]
            {
                @"/directory/that/exists/file.txt", @"/directory/that/exists/with/a/file/that/exists",
                Path.Combine("with", "a", "file", "that", "exists")
            };

            #endregion Unix-style paths

            #region Unix-style Windows paths

            yield return new[]
            {
                @"C:/directory/that/exists", @"C:/directory/that/does/not/exist",
                Path.Combine("..", "does", "not", "exist")
            };

            yield return new[]
            {
                @"C:/directory/that/exists", @"C:/directory/that/does/not/exist/",
                Path.Combine("..", "does", "not", "exist") + Path.DirectorySeparatorChar
            };

            yield return new[]
            {
                @"C:/directory/that/exists/", @"C:/directory/that/does/not/exist",
                Path.Combine("..", "does", "not", "exist")
            };

            yield return new[]
            {
                @"C:/directory/that/exists/", @"C:/directory/that/does/not/exist/",
                Path.Combine("..", "does", "not", "exist") + Path.DirectorySeparatorChar
            };

            yield return new[]
            {
                @"C:/directory/that/exists", @"C:/directory/that/exists/with/a/file/that/exists",
                Path.Combine("with", "a", "file", "that", "exists")
            };

            yield return new[]
            {
                @"C:/directory/that/exists/", @"C:/directory/that/exists/with/a/file/that/exists",
                Path.Combine("with", "a", "file", "that", "exists")
            };

            yield return new[]
            {
                @"C:/directory/that/exists/file.txt", @"C:/directory/that/exists/with/a/file/that/exists",
                Path.Combine("with", "a", "file", "that", "exists")
            };

            #endregion Unix-style Windows paths

            #region Windows paths

            yield return new[]
            {
                @"C:\directory\that\exists", @"C:\directory\that\does\not\exist",
                Path.Combine("..", "does", "not", "exist")
            };

            yield return new[]
            {
                @"C:\directory\that\exists", @"C:\directory\that\does\not\exist\",
                Path.Combine("..", "does", "not", "exist") + Path.DirectorySeparatorChar
            };

            yield return new[]
            {
                @"C:\directory\that\exists\", @"C:\directory\that\does\not\exist",
                Path.Combine("..", "does", "not", "exist")
            };

            yield return new[]
            {
                @"C:\directory\that\exists\", @"C:\directory\that\does\not\exist\",
                Path.Combine("..", "does", "not", "exist") + Path.DirectorySeparatorChar
            };

            yield return new[]
            {
                @"C:\directory\that\exists", @"C:\directory\that\exists\with\a\file\that\exists",
                Path.Combine("with", "a", "file", "that", "exists")
            };

            yield return new[]
            {
                @"C:\directory\that\exists\", @"C:\directory\that\exists\with\a\file\that\exists",
                Path.Combine("with", "a", "file", "that", "exists")
            };

            yield return new[]
            {
                @"C:\directory\that\exists\file.txt", @"C:/directory/that/exists/with/a/file/that/exists",
                Path.Combine("with", "a", "file", "that", "exists")
            };

            #endregion Windows paths
        }

        [TestCaseSource(nameof(RelativePathTestCases))]
        public void TestRelativePath(string from, string to, string expected)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.AreEqual(expected, FileSystemHelper.RelativePath(from, to));
        }
    }
}
