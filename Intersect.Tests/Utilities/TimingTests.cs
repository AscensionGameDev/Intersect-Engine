﻿using Intersect.Utilities;
using NUnit.Framework;
using System.Diagnostics;
using Intersect.Framework.Core;

namespace Intersect.Tests.Server
{
    [TestFixture]
    public partial class TimingTests
    {
        [Test]
        public void TestRealTimeMs()
        {
            var timing = new Timing();
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var expected = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            var actual = timing.MillisecondsUtc;
            stopwatch.Stop();

            var errorDelta = (long) Math.Ceiling(stopwatch.Elapsed.TotalMilliseconds);

            Assert.IsTrue(
                Math.Abs(expected - actual) <= errorDelta,
                $"Expected difference to be less than {errorDelta}ms but was {Math.Abs(expected - actual)}ms."
            );
        }
    }
}
