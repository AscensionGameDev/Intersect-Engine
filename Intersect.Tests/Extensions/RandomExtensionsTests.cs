using System;
using MathNet.Numerics.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intersect.Extensions
{
    [TestClass]
    public class RandomExtensionsTests
    {
        [TestMethod]
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

        [TestMethod]
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

                Assert.AreEqual(value % maximum, random.NextLong(maximum), $"value={value}, maximum={maximum}, %={value % maximum}, {random.NextLong(maximum)}");
            }
        }

        [TestMethod]
        public void NextLongMinimumMaximumTest()
        {
            var random = new MockRandom();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.NextLong(1, 0));
        }
        [TestMethod]
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

        [TestMethod]
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
                var maximum = unchecked((ulong)byteGenerator.NextInt64());

                Assert.AreEqual(value % maximum, random.NextULong(maximum), $"value={value}, maximum={maximum}, %={value % maximum}, {random.NextULong(maximum)}");
            }
        }

        [TestMethod]
        public void NextULongMinimumMaximumTest()
        {
            var random = new MockRandom();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.NextULong(1, 0));
        }

        [TestMethod]
        public void NextDecimalMaximumTest()
        {
            var random = new MockRandom();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.NextDecimal(-1));
        }

        [TestMethod]
        public void NextDecimalMinimumMaximumTest()
        {
            var random = new MockRandom();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.NextDecimal(1, 0));
        }

        [TestMethod]
        public void NextDoubleMaximumTest()
        {
            var random = new MockRandom();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.NextDouble(-1));
        }

        [TestMethod]
        public void NextDoubleMinimumMaximumTest()
        {
            var random = new MockRandom();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.NextDouble(1, 0));
        }

        [TestMethod]
        public void NextFloatTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void NextFloatMaximumTest()
        {
            var random = new MockRandom();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.NextFloat(-1));
        }

        [TestMethod]
        public void NextFloatMinimumMaximumTest()
        {
            var random = new MockRandom();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.NextFloat(1, 0));
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

        public override double NextDouble()
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