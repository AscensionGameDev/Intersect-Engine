using System;
using System.Diagnostics;

using Intersect.Extensions;
using Intersect.Server.Core;

using NUnit.Framework;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Intersect.Tests.Server
{

    [TestFixture]
    public class ServerTimingTests
    {

        [Test]
        public void TestRealTimeMs()
        {
            var timing = new ServerTiming();
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var expected = (long) DateTime.UtcNow.AsUnixTimeSpan().TotalMilliseconds;
            var actual = timing.RealTimeMs;
            stopwatch.Stop();

            var errorDelta = (long) Math.Ceiling(stopwatch.Elapsed.TotalMilliseconds);

            Assert.IsTrue(
                Math.Abs(expected - actual) <= errorDelta,
                $"Expected difference to be less than {errorDelta}ms but was {Math.Abs(expected - actual)}ms."
            );
        }

        [Test]
        public void TestStopwatchNotNull()
        {
            var timing = new ServerTiming();
            Assert.IsNotNull(timing.Stopwatch);
        }

        [Test]
        public void TestTimeMs()
        {
            var timing = new ServerTiming();
            timing.Stopwatch.Stop();
            Assert.AreEqual(timing.Stopwatch.ElapsedMilliseconds, timing.TimeMs);
        }

    }

}
