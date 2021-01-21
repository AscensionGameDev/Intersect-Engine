using System;
using System.Collections.Generic;

using NUnit.Framework;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Intersect.Extensions
{
    [TestFixture]
    public class DateTimeExtensionsTests
    {
        private static IEnumerable<object[]> Data
        {
            get
            {
                yield return new object[] {new DateTime(0, DateTimeKind.Unspecified)};
                yield return new object[] {new DateTime(0, DateTimeKind.Local)};
                yield return new object[] {new DateTime(0, DateTimeKind.Utc)};

                yield return new object[] {new DateTime(DateTime.MinValue.Ticks, DateTimeKind.Unspecified)};
                yield return new object[] {new DateTime(DateTime.MinValue.Ticks, DateTimeKind.Local)};
                yield return new object[] {new DateTime(DateTime.MinValue.Ticks, DateTimeKind.Utc)};

                yield return new object[] {new DateTime(DateTime.MaxValue.Ticks, DateTimeKind.Unspecified)};
                yield return new object[] {new DateTime(DateTime.MaxValue.Ticks, DateTimeKind.Local)};
                yield return new object[] {new DateTime(DateTime.MaxValue.Ticks, DateTimeKind.Utc)};

                var random = new Random();

                yield return new object[]
                {
                    new DateTime(
                        random.NextLong(DateTime.MinValue.Ticks, DateTime.MaxValue.Ticks), DateTimeKind.Unspecified
                    )
                };

                yield return new object[]
                {
                    new DateTime(random.NextLong(DateTime.MinValue.Ticks, DateTime.MaxValue.Ticks), DateTimeKind.Local)
                };

                yield return new object[]
                    {new DateTime(random.NextLong(DateTime.MinValue.Ticks, DateTime.MaxValue.Ticks), DateTimeKind.Utc)};
            }
        }

        [TestCaseSource(nameof(Data))]
        public void TestClone(DateTime dateTime)
        {
            var clone = dateTime.Clone();
            Assert.AreEqual(dateTime.Ticks, clone.Ticks);
            Assert.AreEqual(dateTime.Kind, clone.Kind);
        }

        [Ignore(@"No working test implementation due to environment")] // TODO: This entire test is a pain to implement
        [TestCaseSource(nameof(Data))]
        public void TestConvertKind(DateTime dateTime)
        {
            var convertedLocal = dateTime.ConvertKind(DateTimeKind.Local);
            var convertedUnspecified = dateTime.ConvertKind(DateTimeKind.Unspecified);
            var convertedUtc = dateTime.ConvertKind(DateTimeKind.Utc);
            var tickOffset = DateTimeOffset.Now.Offset.Ticks;

            switch (dateTime.Kind)
            {
                case DateTimeKind.Local:
                    Assert.AreEqual(dateTime, convertedLocal);
                    Assert.AreEqual(DateTimeKind.Unspecified, convertedUnspecified.Kind);
                    Assert.AreEqual(dateTime.Ticks, convertedUnspecified.Ticks);
                    Assert.AreEqual(DateTimeKind.Utc, convertedUtc.Kind);
                    Assert.AreEqual(dateTime.Ticks, convertedUnspecified.Ticks);

                    break;

                case DateTimeKind.Unspecified:
                    // TODO: Figure out how this should be tested when DST is active in the local timezone
                    //Assert.AreEqual(dateTime, convertedUnspecified);
                    //Assert.AreEqual(DateTimeKind.Local, convertedLocal.Kind);
                    //Assert.AreEqual(dateTime.Ticks, convertedLocal.Ticks);
                    //Assert.AreEqual(DateTimeKind.Utc, convertedUtc.Kind);
                    //Assert.AreEqual(dateTime.Ticks, convertedUtc.Ticks - DateTimeOffset.Now.Offset.Ticks);
                    break;

                case DateTimeKind.Utc:
                    Assert.AreEqual(dateTime, convertedUtc);
                    Assert.AreEqual(DateTimeKind.Local, convertedLocal.Kind);
                    Assert.AreEqual(dateTime.Ticks, convertedLocal.Ticks);
                    Assert.AreEqual(DateTimeKind.Unspecified, convertedUnspecified.Kind);
                    Assert.AreEqual(dateTime.Ticks, convertedUnspecified.Ticks);

                    break;

                default:
                    throw new IndexOutOfRangeException();
            }
        }

        [Test]
        public void TestUnixEpoch()
        {
            var unixEpoch = DateTimeExtensions.UnixEpoch;
            Assert.AreEqual(1970, unixEpoch.Year);
            Assert.AreEqual(1, unixEpoch.Month);
            Assert.AreEqual(1, unixEpoch.Day);
            Assert.AreEqual(0, unixEpoch.Hour);
            Assert.AreEqual(0, unixEpoch.Minute);
            Assert.AreEqual(0, unixEpoch.Second);
            Assert.AreEqual(0, unixEpoch.Millisecond);
        }
    }
}
