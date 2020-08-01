using Intersect.Extensions;
using Intersect.Utilities;

using NUnit.Framework;

using System;
using System.Diagnostics;

namespace Intersect.Tests.Server
{
    [TestFixture]
    public class TimingTests
    {
        [Test]
        public void TestRealTimeMs()
        {
            var timing = new Timing();
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var expected = (long) DateTime.UtcNow.AsUnixTimeSpan().TotalMilliseconds;
            var actual = timing.MillisecondsUTC;
            stopwatch.Stop();

            var errorDelta = (long) Math.Ceiling(stopwatch.Elapsed.TotalMilliseconds);

            Assert.IsTrue(
                Math.Abs(expected - actual) <= errorDelta,
                $"Expected difference to be less than {errorDelta}ms but was {Math.Abs(expected - actual)}ms."
            );
        }
    }
}
