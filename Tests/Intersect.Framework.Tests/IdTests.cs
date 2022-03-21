using System.Text.Json;

using NUnit.Framework;

namespace Intersect.Framework.Tests
{
    [TestFixture]
    public class IdTests
    {
        internal class Sample
        {
            public Id<Sample> Id { get; set; }
        }

        [Test]
        public void TestCastGuidDefault()
        {
            var sample = new Sample { Id = default };
            Assert.AreEqual(default(Guid), (Guid)sample.Id);
        }

        [Test]
        public void TestCastGuidNew()
        {
            var expected = Guid.NewGuid();
            var sample = new Sample { Id = new Id<Sample>(expected) };
            Assert.AreEqual(expected, (Guid)sample.Id);
        }

        [Test]
        public void TestSerializeJson()
        {
            var sample = new Sample { Id = default };
            var json = JsonSerializer.Serialize(sample);
            Assert.AreEqual("{\"Id\":\"00000000-0000-0000-0000-000000000000\"}", json);
        }

        [Test]
        public void TestDeserializeJson()
        {
            var sample = JsonSerializer.Deserialize<Sample>("{\"Id\":\"00000000-0000-0000-0000-000000000000\"}");
            Assert.AreEqual(default(Id<Sample>), sample?.Id);
        }
    }
}
