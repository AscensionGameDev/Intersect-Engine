using System;

using Intersect.Extensions;

using NUnit.Framework;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Intersect.Utilities
{

    [TestFixture]
    public class MathHelperTests
    {

        [Test]
        public void ClampDecimal()
        {
            var random = new Random();
            var i = 0;
            while (++i < 101)
            {
                var val = random.NextDecimal(decimal.MinValue, decimal.MaxValue);
                var min = random.NextDecimal(decimal.MinValue, decimal.MaxValue);
                var max = random.NextDecimal(decimal.MinValue, decimal.MaxValue);
                if (min > max)
                {
                    ValueUtils.Swap(ref min, ref max);
                }

                var expected = val;
                if (val < min)
                {
                    expected = min;
                }

                if (val > max)
                {
                    expected = max;
                }

                Assert.AreEqual(
                    expected, MathHelper.Clamp(val, min, max), $"val={val}, min={min}, max={max}, expected={expected}"
                );
            }

            Assert.IsTrue(i > 100);
        }

        [Test]
        public void ClampDouble()
        {
            var random = new Random();
            var i = 0;
            while (++i < 101)
            {
                var val = random.NextDouble(double.MinValue, double.MaxValue);
                var min = random.NextDouble(double.MinValue, double.MaxValue);
                var max = random.NextDouble(double.MinValue, double.MaxValue);
                if (min > max)
                {
                    ValueUtils.Swap(ref min, ref max);
                }

                var expected = val;
                if (val < min)
                {
                    expected = min;
                }

                if (val > max)
                {
                    expected = max;
                }

                Assert.AreEqual(
                    expected, MathHelper.Clamp(val, min, max), $"val={val}, min={min}, max={max}, expected={expected}"
                );
            }

            Assert.IsTrue(i > 100);
        }

        [Test]
        public void ClampLong()
        {
            var random = new Random();
            var i = 0;
            while (++i < 101)
            {
                var val = random.NextLong(long.MinValue, long.MaxValue);
                var min = random.NextLong(long.MinValue, long.MaxValue);
                var max = random.NextLong(long.MinValue, long.MaxValue);
                if (min > max)
                {
                    ValueUtils.Swap(ref min, ref max);
                }

                var expected = val;
                if (val < min)
                {
                    expected = min;
                }

                if (val > max)
                {
                    expected = max;
                }

                Assert.AreEqual(
                    expected, MathHelper.Clamp(val, min, max), $"val={val}, min={min}, max={max}, expected={expected}"
                );
            }

            Assert.IsTrue(i > 100);
        }

        [Test]
        public void ClampULong()
        {
            var random = new Random();
            var i = 0;
            while (++i < 101)
            {
                var val = random.NextULong(ulong.MinValue, ulong.MaxValue);
                var min = random.NextULong(ulong.MinValue, ulong.MaxValue);
                var max = random.NextULong(ulong.MinValue, ulong.MaxValue);
                if (min > max)
                {
                    ValueUtils.Swap(ref min, ref max);
                }

                var expected = val;
                if (val < min)
                {
                    expected = min;
                }

                if (val > max)
                {
                    expected = max;
                }

                Assert.AreEqual(
                    expected, MathHelper.Clamp(val, min, max), $"val={val}, min={min}, max={max}, expected={expected}"
                );
            }

            Assert.IsTrue(i > 100);
        }

    }

}
