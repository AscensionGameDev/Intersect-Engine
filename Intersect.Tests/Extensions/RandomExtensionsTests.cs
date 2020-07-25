using System;

using MathNet.Numerics.Random;

using NUnit.Framework;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Intersect.Extensions
{

    [TestFixture]
    public class RandomExtensionsTests
    {

        [Test]
        public void NextDecimalMaximumTest()
        {
            var random = new MockRandom();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.NextDecimal(-1));
        }

        [Test]
        public void NextDecimalMinimumMaximumTest()
        {
            var random = new MockRandom();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.NextDecimal(1, 0));
        }

        [Test]
        public void NextDoubleMaximumTest()
        {
            var random = new MockRandom();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.NextDouble(-1));
        }

        [Test]
        public void NextDoubleMinimumMaximumTest()
        {
            var random = new MockRandom();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.NextDouble(1, 0));
        }

        [Test]
        public void NextDoubleTest()
        {
            var random = new MockRandom
            {
                MockNextDouble = double.NegativeInfinity
            };

            var nextDouble = random.NextDouble();
            Assert.IsTrue(double.IsNegativeInfinity(nextDouble), $@"Expected negative infinity, got {nextDouble}.");
        }

        [Test]
        public void NextFloatMaximumThrowsExceptionWhenNegativeTest()
        {
            var random = new MockRandom();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.NextFloat(-1));
        }

        [Test]
        public void NextFloatMaximumTest()
        {
            var random = new MockRandom
            {
                MockNextDouble = float.NegativeInfinity
            };
            var nextFloat = random.NextFloat(1);
            Assert.IsTrue(float.IsNegativeInfinity(nextFloat), $@"Expected negative infinity, got {nextFloat}.");
        }

        [Test]
        public void NextFloatMinimumThrowsExceptionWhenMaximumLessThanMinimumTest()
        {
            var random = new MockRandom();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.NextFloat(1, 0));
        }

        [Test]
        public void NextFloatMinimumMaximumTest()
        {
            var random = new MockRandom
            {
                MockNextDouble = float.NegativeInfinity
            };
            var nextFloat = random.NextFloat(0, 1);
            Assert.IsTrue(float.IsNegativeInfinity(nextFloat), $@"Expected negative infinity, got {nextFloat}.");
        }

        [Test]
        public void NextFloatTest()
        {
            var random = new MockRandom
            {
                MockNextDouble = float.NegativeInfinity
            };

            var nextFloat = random.NextFloat();
            Assert.IsTrue(float.IsNegativeInfinity(nextFloat), $@"Expected negative infinity, got {nextFloat}.");
        }

        [Test]
        public void NextLongMaximumTest()
        {
            var random = new MockRandom();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.NextLong(-1));

            var byteGenerator = new Random();

            var buffer = new byte[8];
            for (var i = 0; i < 100; i++)
            {
                byteGenerator.NextBytes(buffer);
                random.MockNextBytes = buffer;

                var value = BitConverter.ToInt64(buffer, 0);
                var maximum = byteGenerator.NextInt64();

                Assert.AreEqual(
                    value % maximum, random.NextLong(maximum),
                    $"value={value}, maximum={maximum}, %={value % maximum}, {random.NextLong(maximum)}"
                );
            }
        }

        [Test]
        public void NextLongMinimumMaximumTest()
        {
            var random = new MockRandom();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.NextLong(1, 0));
        }

        [Test]
        public void NextLongTest()
        {
            var byteGenerator = new Random();
            var random = new MockRandom();
            var buffer = new byte[8];
            for (var i = 0; i < 100; i++)
            {
                byteGenerator.NextBytes(buffer);
                random.MockNextBytes = buffer;
                Assert.AreEqual(BitConverter.ToInt64(buffer, 0), random.NextLong());
            }
        }

        [Test]
        public void NextULongMaximumTest()
        {
            var random = new MockRandom();

            var byteGenerator = new Random();
            var buffer = new byte[8];
            for (var i = 0; i < 100; i++)
            {
                byteGenerator.NextBytes(buffer);
                random.MockNextBytes = buffer;

                var value = BitConverter.ToUInt64(buffer, 0);
                var maximum = unchecked((ulong) byteGenerator.NextInt64());

                Assert.AreEqual(
                    value % maximum, random.NextULong(maximum),
                    $"value={value}, maximum={maximum}, %={value % maximum}, {random.NextULong(maximum)}"
                );
            }
        }

        [Test]
        public void NextULongMinimumMaximumTest()
        {
            var random = new MockRandom();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.NextULong(1, 0));
        }

        [Test]
        public void NextULongTest()
        {
            var byteGenerator = new Random();
            var random = new MockRandom();
            var buffer = new byte[8];
            for (var i = 0; i < 100; i++)
            {
                byteGenerator.NextBytes(buffer);
                random.MockNextBytes = buffer;
                Assert.AreEqual(BitConverter.ToUInt64(buffer, 0), random.NextULong());
            }
        }

    }

    internal class MockRandom : Random
    {

        internal byte[] MockNextBytes { private get; set; }

        internal int MockNext { get; set; }

        internal double MockNextDouble { get; set; }

        public override int Next()
        {
            return MockNext;
        }

        protected override double Sample()
        {
            return MockNextDouble;
        }

        public override void NextBytes(byte[] buffer)
        {
            for (var i = 0; i < buffer.Length; ++i)
            {
                if (MockNextBytes == null || i >= MockNextBytes.Length)
                {
                    buffer[i] = 0;

                    continue;
                }

                buffer[i] = MockNextBytes[i];
            }
        }

    }

}
