using NUnit.Framework;

namespace Intersect.Tests.Server
{

    [TestFixture]
    public partial class Stub
    {

        [Test]
        public void TestStub()
        {
            // Needed so NUnit doesn't return -2
            // TODO: Remove this when there are actual tests in this assembly
            Assert.That(0, Is.EqualTo(0));
        }

    }

}
