
using Intersect.Framework.Randomization;

using Moq;

using NUnit.Framework;

namespace Intersect.Framework.Tests.Randomization;

[TestFixture]
public class RandomExtensionsTest
{
    [SetUp]
    public void Setup()
    {

    }

    private static Random MockRandom(byte[] data)
    {
        var mock = new Mock<Random>();
        mock.Setup(r => r.NextBytes(It.IsAny<byte[]>())).Callback((byte[] bytes) => data.AsSpan().CopyTo(bytes));
        var random = mock.Object;
        return random;
    }

    static IEnumerable<TestCaseData> TestNextByteData()
    {
        for (var value = 0; value < 256; ++value)
        {
            yield return new TestCaseData(new[] { (byte)value }).SetName(nameof(TestNextByte)).Returns((byte)value);
        }
    }

    [TestCaseSource(nameof(TestNextByteData))]
    public object TestNextByte(byte[] data) => MockRandom(data).NextByte();

    static IEnumerable<TestCaseData> TestNextCharData()
    {
        for (var index = 0; index < 256; ++index)
        {
            var buffer = new byte[sizeof(char)];
            Random.Shared.NextBytes(buffer);
            var value = BitConverter.ToChar(buffer);
            yield return new TestCaseData(buffer).SetName(nameof(TestNextChar)).Returns(value);
        }
    }

    [TestCaseSource(nameof(TestNextCharData))]
    public object TestNextChar(byte[] data) => MockRandom(data).NextChar();

    static IEnumerable<TestCaseData> TestNextDoubleData()
    {
        for (var index = 0; index < 256; ++index)
        {
            var buffer = new byte[sizeof(double)];
            Random.Shared.NextBytes(buffer);
            var value = BitConverter.ToDouble(buffer);
            yield return new TestCaseData(buffer).SetName(nameof(TestNextDouble)).Returns(value);
        }
    }

    [Ignore("BitConverter seems to not be deterministic for floating points.")]
    [TestCaseSource(nameof(TestNextDoubleData))]
    public double TestNextDouble(byte[] data) => MockRandom(data).NextDouble();

    static IEnumerable<TestCaseData> TestNextFloatData()
    {
        for (var index = 0; index < 256; ++index)
        {
            var buffer = new byte[sizeof(float)];
            Random.Shared.NextBytes(buffer);
            var value = BitConverter.ToSingle(buffer);
            yield return new TestCaseData(buffer).SetName(nameof(TestNextFloat)).Returns(value);
        }
    }

    [Ignore("BitConverter seems to not be deterministic for floating points.")]
    [TestCaseSource(nameof(TestNextFloatData))]
    public object TestNextFloat(byte[] data) => MockRandom(data).NextFloat();

    static IEnumerable<TestCaseData> TestNextIntData()
    {
        for (var index = 0; index < 256; ++index)
        {
            var buffer = new byte[sizeof(int)];
            Random.Shared.NextBytes(buffer);
            var value = BitConverter.ToInt32(buffer);
            yield return new TestCaseData(buffer).SetName(nameof(TestNextInt)).Returns(value);
        }
    }

    [TestCaseSource(nameof(TestNextIntData))]
    public object TestNextInt(byte[] data) => MockRandom(data).NextInt();

    static IEnumerable<TestCaseData> TestNextLongData()
    {
        for (var index = 0; index < 256; ++index)
        {
            var buffer = new byte[sizeof(long)];
            Random.Shared.NextBytes(buffer);
            var value = BitConverter.ToInt64(buffer);
            yield return new TestCaseData(buffer).SetName(nameof(TestNextLong)).Returns(value);
        }
    }

    [TestCaseSource(nameof(TestNextLongData))]
    public object TestNextLong(byte[] data) => MockRandom(data).NextLong();

    static IEnumerable<TestCaseData> TestNextSByteData()
    {
        for (var value = 0; value < 256; ++value)
        {
            yield return new TestCaseData(new[] { (sbyte)value }).SetName(nameof(TestNextSByte)).Returns((sbyte)value);
        }
    }

    [TestCaseSource(nameof(TestNextSByteData))]
    public object TestNextSByte(byte[] data) => MockRandom(data).NextSByte();

    static IEnumerable<TestCaseData> TestNextShortData()
    {
        for (var index = 0; index < 256; ++index)
        {
            var buffer = new byte[sizeof(short)];
            Random.Shared.NextBytes(buffer);
            var value = BitConverter.ToInt16(buffer);
            yield return new TestCaseData(buffer).SetName(nameof(TestNextShort)).Returns(value);
        }
    }

    [TestCaseSource(nameof(TestNextShortData))]
    public object TestNextShort(byte[] data) => MockRandom(data).NextShort();

    static IEnumerable<TestCaseData> TestNextUIntData()
    {
        for (var index = 0; index < 256; ++index)
        {
            var buffer = new byte[sizeof(uint)];
            Random.Shared.NextBytes(buffer);
            var value = BitConverter.ToUInt32(buffer);
            yield return new TestCaseData(buffer).SetName(nameof(TestNextUInt)).Returns(value);
        }
    }

    [TestCaseSource(nameof(TestNextUIntData))]
    public object TestNextUInt(byte[] data) => MockRandom(data).NextUInt();

    static IEnumerable<TestCaseData> TestNextULongData()
    {
        for (var index = 0; index < 256; ++index)
        {
            var buffer = new byte[sizeof(ulong)];
            Random.Shared.NextBytes(buffer);
            var value = BitConverter.ToUInt64(buffer);
            yield return new TestCaseData(buffer).SetName(nameof(TestNextULong)).Returns(value);
        }
    }

    [TestCaseSource(nameof(TestNextULongData))]
    public object TestNextULong(byte[] data) => MockRandom(data).NextULong();

    static IEnumerable<TestCaseData> TestNextUShortData()
    {
        for (var index = 0; index < 256; ++index)
        {
            var buffer = new byte[sizeof(ushort)];
            Random.Shared.NextBytes(buffer);
            var value = BitConverter.ToUInt16(buffer);
            yield return new TestCaseData(buffer).SetName(nameof(TestNextUShort)).Returns(value);
        }
    }

    [TestCaseSource(nameof(TestNextUShortData))]
    public object TestNextUShort(byte[] data) => MockRandom(data).NextUShort();
}
