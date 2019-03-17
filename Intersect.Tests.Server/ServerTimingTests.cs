using System;
using System.Diagnostics;

using Intersect.Extensions;
using Intersect.Server;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intersect.Tests.Server
{
    [TestClass]
    public class ServerTimingTests
    {
        [TestMethod]
        public void TestStopwatchNotNull()
        {
            var timing = new ServerTiming();
            Assert.IsNotNull(timing.Stopwatch);
        }

        [TestMethod]
        public void TestTimeMs()
        {
            var timing = new ServerTiming();
            timing.Stopwatch.Stop();
            Assert.AreEqual(timing.Stopwatch.ElapsedMilliseconds, timing.TimeMs);
        }

        [TestMethod]
        public void TestRealTimeMs()
        {
            var timing = new ServerTiming();
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var expected = (long) DateTime.UtcNow.AsUnixTimeSpan().TotalMilliseconds;
            var actual = timing.RealTimeMs;
            stopwatch.Stop();

            var errorDelta = (long) Math.Ceiling(stopwatch.Elapsed.TotalMilliseconds);

            Assert.IsTrue(Math.Abs(expected - actual) <= errorDelta, $"Expected difference to be less than {errorDelta}ms but was {Math.Abs(expected - actual)}ms.");
        }
    }
}
