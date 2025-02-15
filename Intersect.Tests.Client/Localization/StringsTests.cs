using System.Globalization;
using Intersect.Client.Localization;
using NUnit.Framework;

namespace Intersect.Tests.Client.Localization;

[TestFixture]
public class StringsTests
{
    [SetUp]
    public void SetUp()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    }

    [TestCase(1, "1B")]
    [TestCase(999, "999B")]
    [TestCase(1001, "1001B")]
    [TestCase(1234, "1234B")]
    [TestCase(12345, "12.35KB")]
    public void TestFormatBytes(long bytes, string expectedFormattedString)
    {
        var actualFormattedString = Strings.FormatBytes(bytes);
        Assert.That(actualFormattedString, Is.EqualTo(expectedFormattedString));
    }

    [TestCase(1, "1B")]
    [TestCase(999, "999B")]
    [TestCase(1001, "1001B")]
    [TestCase(1234, "1234B")]
    [TestCase(12641, "12.34KiB")]
    [TestCase(12944670, "12.34MiB")]
    [TestCase(13255342817, "12.34GiB")]
    public void TestFormatBits(long bytes, string expectedFormattedString)
    {
        var actualFormattedString = Strings.FormatBits(bytes);
        Assert.That(actualFormattedString, Is.EqualTo(expectedFormattedString));
    }
}